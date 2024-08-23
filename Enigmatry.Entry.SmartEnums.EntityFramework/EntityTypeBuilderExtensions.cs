using System.Linq.Expressions;
using Ardalis.SmartEnum;
using Enigmatry.Entry.SmartEnums.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enigmatry.Entry.SmartEnums.EntityFramework;

[PublicAPI]
public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Configures the entity to have SmartEnum as primary key
    /// </summary>
    /// <param name="builder">Entity type builder</param>
    /// <typeparam name="T">Typeof entity</typeparam>
    /// <typeparam name="TId">Typeof id</typeparam>
    public static void HasEnumId<T, TId>(this EntityTypeBuilder<T> builder)
        where T : EntityWithEnumId<TId> where TId : SmartEnum<TId>
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasSmartEnumConversion()
            .ValueGeneratedNever();
    }

    /// <summary>
    /// Configures the entity to have reference to another entity with SmartEnum as the foreign key
    /// </summary>
    /// <param name="builder">Entity type builder</param>
    /// <param name="referenceSelector">Expression for defining the navigational property</param>
    /// <param name="foreignKeySelector">Expression for defining the foreign key column</param>
    /// <typeparam name="TEntity">Typeof entity</typeparam>
    /// <typeparam name="TReferencedEntity">Typeof referenced table</typeparam>
    /// <typeparam name="TId">Typeof smart enum value</typeparam>
    public static void HasReferenceTableRelationWithEnumAsForeignKey<TEntity, TReferencedEntity, TId>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TReferencedEntity?>> referenceSelector,
        Expression<Func<TEntity, TId>> foreignKeySelector)
        where TEntity : class
        where TReferencedEntity : class
        where TId : SmartEnum<TId>
    {
        builder.Property(foreignKeySelector).HasSmartEnumConversion();
        builder.HasOne(referenceSelector)
            .WithMany()
            .HasForeignKey(((MemberExpression)foreignKeySelector.Body).Member.Name)
            .OnDelete(DeleteBehavior.Restrict);
    }

    /// <summary>
    /// Configures the entity to have nullable reference to another entity with SmartEnum as the foreign key
    /// </summary>
    /// <param name="builder">Entity type builder</param>
    /// <param name="referenceSelector">Expression for defining the navigational property</param>
    /// <param name="foreignKeySelector">Expression for defining the foreign key column</param>
    /// <typeparam name="TEntity">Typeof entity</typeparam>
    /// <typeparam name="TReferencedEntity">Typeof referenced entity</typeparam>
    /// <typeparam name="TId">Typeof smart enum value</typeparam>
    public static void HasNullableReferenceTableRelationWithEnumAsForeignKey<TEntity, TReferencedEntity, TId>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TReferencedEntity?>> referenceSelector,
        Expression<Func<TEntity, TId?>> foreignKeySelector)
        where TEntity : class
        where TReferencedEntity : class
        where TId : SmartEnum<TId>
    {
        builder.Property(foreignKeySelector).HasNullableSmartEnumConversion();
        builder.HasOne(referenceSelector)
            .WithMany()
            .HasForeignKey(((MemberExpression)foreignKeySelector.Body).Member.Name)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
