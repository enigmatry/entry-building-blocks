using Ardalis.SmartEnum;
using Enigmatry.Entry.Core.EntityFramework.Seeding;
using Enigmatry.Entry.SmartEnums.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Enigmatry.Entry.SmartEnums.EntityFramework.Seeding;

/// <summary>
/// Defines seeding for entities with SmartEnum as primary key
/// </summary>
/// <typeparam name="T">Typeof entity</typeparam>
/// <typeparam name="TId">Typeof id</typeparam>
[PublicAPI]
public class EntityWithEnumIdSeeding<T, TId> : ISeeding
    where T : EntityWithEnumId<TId>
    where TId : SmartEnum<TId>
{
    /// <summary>
    /// default entity factory
    /// </summary>
    private readonly Func<TId, object> _entityFactory = id => new { Id = id, id.Name };

    public EntityWithEnumIdSeeding()
    {
    }

    public EntityWithEnumIdSeeding(Func<TId, object> entityFactory)
    {
        _entityFactory = entityFactory;
    }

    public void Seed(ModelBuilder modelBuilder)
    {
        // iterate through all the enum values and add them to the entity
        foreach (var id in SmartEnum<TId>.List)
        {
            modelBuilder.Entity<T>().HasData(_entityFactory(id));
        }
    }
}
