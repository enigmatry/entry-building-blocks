using System;
using System.Linq;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Entities
{
    public static class EntityQueryableExtensions
    {
        public static IQueryable<T> QueryById<T>(this IQueryable<T> query, Guid id) where T : Entity<Guid> =>
            query.Where(e => e.Id == id);

        public static IQueryable<T> QueryById<T>(this IQueryable<T> query, int id) where T : Entity<int> =>
            query.Where(e => e.Id == id);

        public static IQueryable<T> QueryExceptWithId<T>(this IQueryable<T> query, Guid? id) where T : Entity<Guid> =>
            query.Where(e => e.Id != id);

        public static IQueryable<T> QueryExceptWithId<T>(this IQueryable<T> query, int? id) where T : Entity<int> =>
            query.Where(e => e.Id != id);

        public static bool EntityExists<T>(this IQueryable<T> query, Guid id) where T : Entity<Guid> =>
            query.Any(x => x.Id == id);

        public static bool EntityExists<T>(this IQueryable<T> query, int id) where T : Entity<int> =>
            query.Any(x => x.Id == id);

        public static T SingleOrNotFound<T>(this IQueryable<T> query)
        {
            var result = query.SingleOrDefault();
            return result != null ? result : throw new EntityNotFoundException(typeof(T).Name);
        }
    }
}
