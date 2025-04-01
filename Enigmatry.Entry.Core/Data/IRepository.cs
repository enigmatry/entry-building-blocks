using Enigmatry.Entry.Core.Entities;

namespace Enigmatry.Entry.Core.Data;

public interface IRepository<T> : IRepository<T, Guid> where T : EntityWithTypedId<Guid>
{
}

public interface IRepository<T, in TId> : IEntityRepository<T> where T : EntityWithTypedId<TId>
{
    public void Delete(TId id);

    public T? FindById(TId id);

    public Task<T?> FindByIdAsync(TId id);
}
