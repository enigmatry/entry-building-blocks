using Enigmatry.Entry.Core.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Enigmatry.Entry.EntityFramework;

[PublicAPI]
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Apply convention to set ValueGeneratedNever for primary keys of classes deriving from EntityWithGuidId
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    public static void ApplyEntityWithGuidIdConvention(this ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes();
        var entityWithGuidIdTypes = entityTypes.Where(e => typeof(EntityWithGuidId).IsAssignableFrom(e.ClrType));
        foreach (var entityType in entityWithGuidIdTypes)
        {
            var key = entityType.FindPrimaryKey() ??
                      throw new InvalidOperationException($"{entityType} must have a primary key.");
            foreach (var property in key.Properties)
            {
                if (property.Name == nameof(EntityWithGuidId.Id) && property.ClrType == typeof(Guid))
                {
                    property.ValueGenerated = ValueGenerated.Never;
                }
            }
        }
    }

    /// <summary>
    /// Register entities from the assembly specified in the options
    /// </summary>
    /// <param name="modelBuilder">Model Builder</param>
    /// <param name="options">Registration options</param>
    public static void RegisterEntities(this ModelBuilder modelBuilder, EntitiesDbContextOptions options)
    {
        var entitiesAssembly = options.EntitiesAssembly;
        var types = entitiesAssembly.GetTypes();

        var entityTypes = types
            .Where(x => x.IsSubclassOf(typeof(Entity)) && !x.IsAbstract);

        foreach (var type in entityTypes)
        {
            if (options.EntityTypePredicate != null &&
                !options.EntityTypePredicate(type))
            {
                continue;
            }

            modelBuilder.Entity(type);
        }
    }
}
