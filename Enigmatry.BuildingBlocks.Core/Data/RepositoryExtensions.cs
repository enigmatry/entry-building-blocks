using System;
using System.Collections.Generic;
using Enigmatry.BuildingBlocks.Core.Entities;

namespace Enigmatry.BuildingBlocks.Core.Data
{
    public static class RepositoryExtensions
    {
        public static void AddRange<T>(this IEntityRepository<T> repository, IEnumerable<T> entities) where T : Entity
        {
            foreach (T entity in entities)
            {
                repository.Add(entity);
            }
        }

        public static bool EntityExists<T>(this IEntityRepository<T> repository, Guid id) where T : EntityWithTypedId<Guid> =>
            repository.QueryAll().EntityExists(id);

        public static bool EntityExists<T>(this IEntityRepository<T> repository, int id) where T : EntityWithTypedId<int> =>
            repository.QueryAll().EntityExists(id);
    }
}
