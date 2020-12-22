using System;
using System.Collections.Generic;
using Enigmatry.Blueprint.BuildingBlocks.Core.Entities;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Data
{
    public static class RepositoryExtensions
    {
        public static void AddRange<T>(this IRepository<T> repository, IEnumerable<T> entities) where T : EntityBase
        {
            foreach (T entity in entities)
            {
                repository.Add(entity);
            }
        }

        public static bool EntityExists<T>(this IRepository<T> repository, Guid id) where T : Entity<Guid> =>
            repository.QueryAll().EntityExists(id);

        public static bool EntityExists<T>(this IRepository<T> repository, int id) where T : Entity<int> =>
            repository.QueryAll().EntityExists(id);
    }
}
