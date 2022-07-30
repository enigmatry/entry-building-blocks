using Enigmatry.BuildingBlocks.Core.Entities;
using Enigmatry.BuildingBlocks.Core.Helpers;
using Enigmatry.BuildingBlocks.EntityFramework.MediatR;
using Enigmatry.BuildingBlocks.EntityFramework.Security;
using JetBrains.Annotations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.EntityFramework
{
    [UsedImplicitly]
    public abstract class BaseDbContext : DbContext
    {
        private readonly EntitiesDbContextOptions _entitiesDbContextOptions;
        private readonly IDbContextAccessTokenProvider _dbContextAccessTokenProvider;

        public Action<ModelBuilder>? ModelBuilderConfigurator { get; set; }

        protected BaseDbContext(EntitiesDbContextOptions entitiesDbContextOptions, DbContextOptions options,
            IDbContextAccessTokenProvider dbContextAccessTokenProvider) : base(options)
        {
            _entitiesDbContextOptions = entitiesDbContextOptions;
            _dbContextAccessTokenProvider = dbContextAccessTokenProvider;

            SetupManagedServiceIdentityAccessToken();
        }

        private void SetupManagedServiceIdentityAccessToken()
        {
            var accessToken = _dbContextAccessTokenProvider.GetAccessTokenAsync().GetAwaiter().GetResult();
            if (accessToken.HasContent())
            {
                var connection = (SqlConnection)Database.GetDbConnection();
                connection.AccessToken = accessToken;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(_entitiesDbContextOptions.ConfigurationAssembly);

            RegisterEntities(modelBuilder);

            ModelBuilderConfigurator?.Invoke(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        [SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract")]
        private void RegisterEntities(ModelBuilder modelBuilder)
        {
            var entityMethod =
                typeof(ModelBuilder).GetMethods().First(m => m.Name == "Entity" && m.IsGenericMethod);

            var entitiesAssembly = _entitiesDbContextOptions.EntitiesAssembly;
            var types = entitiesAssembly?.GetTypes() ?? Enumerable.Empty<Type>();

            var entityTypes = types
                .Where(x => x.IsSubclassOf(typeof(Entity)) && !x.IsAbstract);

            foreach (var type in entityTypes)
            {
                entityMethod.MakeGenericMethod(type).Invoke(modelBuilder, Array.Empty<object>());
            }
        }

        public override int SaveChanges()
        {
            var task = Task.Run(async () => await SaveChangesAsync());
            return task.GetAwaiter().GetResult();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // we need to gather domain events before saving, so that we include events
            // for deleted entities (otherwise they are lost due to deletion of the object from context)
            var domainEvents = this.GatherDomainEventsFromContext();

            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var saved = await base.SaveChangesAsync(cancellationToken);

            await Dispatch(domainEvents);
            return saved;
        }

        protected abstract Task Dispatch(IEnumerable<DomainEvent> domainEvents);
    }
}
