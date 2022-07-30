using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.Core.Data
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void CancelSaving();
    }
}
