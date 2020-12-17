using System;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Entities
{
    public static class EntityExtensions
    {
        public static T WithNextSequentialId<T>(this T entity) where T : Entity<Guid> =>
            entity.WithId(SequentialGuidGenerator.Generate());

        public static T WithId<T, TId>(this T entity, TId id) where T : Entity<TId>
        {
            entity.Id = id;
            return entity;
        }
    }
}
