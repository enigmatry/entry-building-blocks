// unset

using System.Linq.Expressions;
using Enigmatry.Entry.Core.Entities;

namespace Enigmatry.Entry.Core.Data;

public interface IEntityRepository<T> where T : Entity
{
    public void Add(T entity);

    public void Delete(T entity);

    public void DeleteRange(IEnumerable<T> entities);

    public IQueryable<T> QueryAll();

    public IQueryable<T> QueryAllSkipCache();

    public IQueryable<T> QueryAllIncluding(params Expression<Func<T, object>>[] paths);

    public IQueryable<T> QueryAllSkipCacheIncluding(params Expression<Func<T, object>>[] paths);
}
