using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Enigmatry.Blueprint.BuildingBlocks.Core.Data;
using Enigmatry.Blueprint.BuildingBlocks.Core.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Enigmatry.Blueprint.BuildingBlocks.Infrastructure.EntityFramework
{
    [UsedImplicitly]
    public class EntityFrameworkRepository<T, TId> : IRepository<T, TId> where T : Entity<TId> where TId : struct
    {
        public EntityFrameworkRepository(DbContext context)
        {
            DbContext = context;
            DbSet = context.Set<T>();
        }

        protected DbSet<T> DbSet { get; }

        protected DbContext DbContext { get; }

        public IQueryable<T> QueryAll() => DbSet;

        public IQueryable<T> QueryAllSkipCache() => DbSet.AsNoTracking();

        public IQueryable<T> QueryAllIncluding(params Expression<Func<T, object>>[] paths) =>
            paths.Aggregate(QueryAll(),
                (current, includeProperty) => current.Include(includeProperty));

        public IQueryable<T> QueryAllSkipCacheIncluding(params Expression<Func<T, object>>[] paths) =>
            paths.Aggregate(QueryAllSkipCache(),
                (current, includeProperty) => current.Include(includeProperty));


        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            DbSet.Add(item);
        }

        public void Delete(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            DbSet.Remove(item);
        }

        public void Delete(TId id)
        {
            var item = FindById(id);
            if (item != null)
            {
                Delete(item);
            }
        }

        public void DeleteRange(IEnumerable<T> entities) => DbSet.RemoveRange(entities);

        public T? FindById(TId id) => DbSet.Find(id);

        public async Task<T?> FindByIdAsync(TId id) => await DbSet.FindAsync(id);
    }
}
