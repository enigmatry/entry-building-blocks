using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Entities
{
    public static class EntityQueryableExtensions
    {
        public static IQueryable<T> QueryById<T>(this IQueryable<T> query, Guid id) where T : EntityWithTypedId<Guid> =>
            query.Where(e => e.Id == id);

        public static IQueryable<T> QueryById<T>(this IQueryable<T> query, int id) where T : EntityWithTypedId<int> =>
            query.Where(e => e.Id == id);

        public static IQueryable<T> QueryExceptWithId<T>(this IQueryable<T> query, Guid? id) where T : EntityWithTypedId<Guid> =>
            query.Where(e => e.Id != id);

        public static IQueryable<T> QueryExceptWithId<T>(this IQueryable<T> query, int? id) where T : EntityWithTypedId<int> =>
            query.Where(e => e.Id != id);

        public static IQueryable<T> QueryByIds<T>(this IQueryable<T> query, IEnumerable<Guid> ids)
            where T : EntityWithTypedId<Guid> =>
            query.Where(e => ids.Contains(e.Id));

        public static IQueryable<T> QueryByIds<T>(this IQueryable<T> query, IEnumerable<int> ids)
            where T : EntityWithTypedId<int> =>
            query.Where(e => ids.Contains(e.Id));

        public static bool EntityExists<T>(this IQueryable<T> query, Guid id) where T : EntityWithTypedId<Guid> =>
            query.Any(x => x.Id == id);

        public static bool EntityExists<T>(this IQueryable<T> query, int id) where T : EntityWithTypedId<int> =>
            query.Any(x => x.Id == id);

        public static T SingleOrNotFound<T>(this IQueryable<T> query)
        {
            var result = query.SingleOrDefault();
            return result != null ? result : throw new EntityNotFoundException(typeof(T).Name);
        }
    }
}
