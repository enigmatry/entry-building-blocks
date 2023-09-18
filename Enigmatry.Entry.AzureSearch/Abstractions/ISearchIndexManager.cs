using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.Entry.AzureSearch.Abstractions;

public interface ISearchIndexManager<T>
{
    Task<bool> DeleteIndex(CancellationToken cancellationToken = default);
    Task<bool> RecreateIndex(CancellationToken cancellationToken = default);
}
