using System;

namespace Enigmatry.Entry.Core.Entities
{
    public static class EntityExtensions
    {
        public static T WithNextSequentialId<T>(this T entity) where T : EntityWithTypedId<Guid> =>
            entity.WithId(SequentialGuidGenerator.Generate());

        public static T WithId<T, TId>(this T entity, TId id) where T : EntityWithTypedId<TId>
        {
            entity.Id = id;
            return entity;
        }
    }
}
