using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Enigmatry.Entry.EntityFramework;

[PublicAPI]
public class EntitiesDbContextOptions
{
    /// <summary>
    /// Assembly containing the entity type configurations to be registered with the DbContext.
    /// </summary>
    public Assembly ConfigurationAssembly { get; set; } = default!;

    /// <summary>
    /// Predicate to filter the entity type configuration types to be registered with the DbContext.
    /// </summary>
    public Func<Type, bool>? ConfigurationTypePredicate { get; set; }

    /// <summary>
    /// Assembly containing the entities to be registered with the DbContext.
    /// </summary>
    public Assembly EntitiesAssembly { get; set; } = default!;

    /// <summary>
    /// Predicate to filter the entity types to be registered with the DbContext.
    /// </summary>
    public Func<Type, bool>? EntityTypePredicate { get; set; }
}
