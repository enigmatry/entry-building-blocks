using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.Entry.BlobStorage
{
    public interface IBlobStorage
    {
        string Name { get; }
        string BuildResourcePath(string relativeUri);
        Task<bool> ExistsAsync(string relativeUri, CancellationToken cancellationToken = default);
        Task AddAsync(string relativeUri, Stream content, bool @override = false, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(string relativeUri, CancellationToken cancellationToken = default);
        Task<Stream> GetAsync(string relativeUri, CancellationToken cancellationToken = default);
        Task CopyAsync(string relativeUri, Uri absoluteUri, CancellationToken cancellationToken);
    }
}
