using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Enigmatry.BuildingBlocks.Core.Entities;
using Enigmatry.BuildingBlocks.Core.Helpers;
using Enigmatry.BuildingBlocks.EntityFramework.MediatR;
using Enigmatry.BuildingBlocks.EntityFramework.Security;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Enigmatry.BuildingBlocks.EntityFramework
{
    [UsedImplicitly]
    public abstract class EntitiesDbContext : DbContext
    {
        private readonly EntitiesDbContextOptions _entitiesDbContextOptions;
        private readonly IMediator _mediator;
        private readonly ILogger<EntitiesDbContext> _logger;
        private readonly IDbContextAccessTokenProvider _dbContextAccessTokenProvider;

        public Action<ModelBuilder>? ModelBuilderConfigurator { get; set; }

        protected EntitiesDbContext(EntitiesDbContextOptions entitiesDbContextOptions, DbContextOptions options, IMediator mediator,
            ILogger<EntitiesDbContext> logger, IDbContextAccessTokenProvider dbContextAccessTokenProvider) : base(
            options)
        {
            _entitiesDbContextOptions = entitiesDbContextOptions;
            _mediator = mediator;
            _logger = logger;
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

        private void RegisterEntities(ModelBuilder modelBuilder)
        {
            MethodInfo entityMethod =
                typeof(ModelBuilder).GetMethods().First(m => m.Name == "Entity" && m.IsGenericMethod);

            Assembly? entitiesAssembly = _entitiesDbContextOptions.EntitiesAssembly;
            var types = entitiesAssembly != null ? entitiesAssembly.GetTypes() : Enumerable.Empty<Type>();

            IEnumerable<Type> entityTypes = types
                .Where(x => x.IsSubclassOf(typeof(Entity)) && !x.IsAbstract);

            foreach (Type type in entityTypes)
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
            IEnumerable<DomainEvent> domainEvents = this.GatherDomainEventsFromContext();

            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var saved = await base.SaveChangesAsync(cancellationToken);

            await _mediator.DispatchDomainEventsAsync(domainEvents, _logger);

            return saved;
        }
    }
}
