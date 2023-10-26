using Enigmatry.Entry.Core.Entities;
using Enigmatry.Entry.Core.Helpers;
using Enigmatry.Entry.EntityFramework.Security;
using JetBrains.Annotations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Enigmatry.Entry.EntityFramework;

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
        if (!accessToken.HasContent())
        {
            return;
        }

        var connection = (SqlConnection)Database.GetDbConnection();
        connection.AccessToken = accessToken;
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
}
