using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Enigmatry.BuildingBlocks.Core.Data;
using Enigmatry.BuildingBlocks.Core.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Enigmatry.BuildingBlocks.EntityFramework
{
    [UsedImplicitly]
    public class EntityFrameworkRepository<T> : EntityFrameworkRepository<T, Guid>, IRepository<T> where T : EntityWithTypedId<Guid>
    {
        public EntityFrameworkRepository(DbContext context) : base(context)
        {
        }
    }

    [UsedImplicitly]
    public class EntityFrameworkRepository<T, TId> : IRepository<T, TId> where T : EntityWithTypedId<TId>
    {
        public EntityFrameworkRepository(DbContext context)
        {
            DbContext = context;
            DbSet = context.Set<T>();
        }

        protected DbSet<T> DbSet { get; }

        protected DbContext DbContext { get; }

        public virtual IQueryable<T> QueryAll() => DbSet;

        public virtual IQueryable<T> QueryAllSkipCache() => DbSet.AsNoTracking();

        public virtual IQueryable<T> QueryAllIncluding(params Expression<Func<T, object>>[] paths) =>
            paths.Aggregate(QueryAll(),
                (current, includeProperty) => current.Include(includeProperty));

        public virtual IQueryable<T> QueryAllSkipCacheIncluding(params Expression<Func<T, object>>[] paths) =>
            paths.Aggregate(QueryAllSkipCache(),
                (current, includeProperty) => current.Include(includeProperty));


        public virtual void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            DbSet.Add(entity);
        }

        public virtual void Delete(TId id)
        {
            var item = FindById(id);
            if (item != null)
            {
                Delete(item);
            }
        }


#pragma warning disable CA1725 // Parameter names should match base declaration
        public virtual void Delete(T item)
#pragma warning restore CA1725 // Parameter names should match base declaration
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            DbSet.Remove(item);
        }

        public virtual void DeleteRange(IEnumerable<T> entities) => DbSet.RemoveRange(entities);

        public virtual T? FindById(TId id) => DbSet.Find(id);

        public virtual async Task<T?> FindByIdAsync(TId id) => await DbSet.FindAsync(id);
    }
}
