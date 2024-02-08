using Enigmatry.Entry.Core.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Enigmatry.Entry.EntityFramework;

[UsedImplicitly]
public abstract class BaseDbContext : DbContext
{
    private readonly EntitiesDbContextOptions _entitiesDbContextOptions;

    public Action<ModelBuilder>? ModelBuilderConfigurator { get; set; }

    protected BaseDbContext(EntitiesDbContextOptions entitiesDbContextOptions, DbContextOptions options) : base(options)
    {
        _entitiesDbContextOptions = entitiesDbContextOptions;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(_entitiesDbContextOptions.ConfigurationAssembly,
            _entitiesDbContextOptions.ConfigurationTypePredicate);

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
            if (_entitiesDbContextOptions.EntityTypePredicate != null &&
                !_entitiesDbContextOptions.EntityTypePredicate(type))
            {
                continue;
            }

            entityMethod.MakeGenericMethod(type).Invoke(modelBuilder, []);
        }
    }
}
