using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Enigmatry.Blueprint.BuildingBlocks.Core.Entities;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Data
{
    public interface IRepository<T> where T : EntityBase
    {
        void Add(T item);

        void Delete(T item);

        void DeleteRange(IEnumerable<T> entities);

        IQueryable<T> QueryAll();

        IQueryable<T> QueryAllSkipCache();

        IQueryable<T> QueryAllIncluding(params Expression<Func<T, object>>[] paths);

        IQueryable<T> QueryAllSkipCacheIncluding(params Expression<Func<T, object>>[] paths);
    }

    public interface IRepository<T, in TId> : IRepository<T> where T : Entity<TId>
    {
        void Delete(TId id);

        T? FindById(TId id);

        Task<T?> FindByIdAsync(TId id);
    }
}
