using Ardalis.SmartEnum;
using Enigmatry.Entry.SmartEnums.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enigmatry.Entry.SmartEnums.EntityFramework;

/// <summary>
/// Defines the configuration for entities with SmartEnum as primary key
/// </summary>
/// <param name="nameMaxLength">Max lenght of the Name column in the database</param>
/// <typeparam name="TEntity">Typeof entity</typeparam>
/// <typeparam name="TId">Typeof id</typeparam>
[UsedImplicitly]
public abstract class EntityWithEnumIdConfiguration<TEntity, TId>(int nameMaxLength) : IEntityTypeConfiguration<TEntity>
    where TEntity : EntityWithEnumId<TId> where TId : SmartEnum<TId>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasEnumId<TEntity, TId>();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(nameMaxLength);
    }
}
