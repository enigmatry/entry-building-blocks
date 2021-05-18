using System;
using System.Threading.Tasks;
using Enigmatry.BuildingBlocks.Core.Entities;

namespace Enigmatry.BuildingBlocks.Core.Data
{
    public interface IRepository<T> : IRepository<T, Guid> where T : EntityWithTypedId<Guid>
    {
    }

    public interface IRepository<T, in TId> : IEntityRepository<T> where T : EntityWithTypedId<TId>
    {
        void Delete(TId id);

        T? FindById(TId id);

        Task<T?> FindByIdAsync(TId id);
    }
}
