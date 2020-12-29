// unset

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Enigmatry.Blueprint.BuildingBlocks.Core.Entities;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Data
{
    public interface IEntityRepository<T> where T : Entity
    {
        void Add(T entity);

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entities);

        IQueryable<T> QueryAll();

        IQueryable<T> QueryAllSkipCache();

        IQueryable<T> QueryAllIncluding(params Expression<Func<T, object>>[] paths);

        IQueryable<T> QueryAllSkipCacheIncluding(params Expression<Func<T, object>>[] paths);
    }
}
