using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.Blueprint.BuildingBlocks.BlobStorage
{
    public interface IBlobStorage
    {
        string BuildResourcePath(string path);
        Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);
        Task AddAsync(string path, Stream content, bool @override = false, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(string path, CancellationToken cancellationToken = default);
        Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default);
    }
}
