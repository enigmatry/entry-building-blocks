using Enigmatry.Entry.BlobStorage.Models;
using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage;

[PublicAPI]
public interface IBlobStorage
{
    string Name { get; }
    string BuildResourcePath(string relativeUri);
    Task<bool> ExistsAsync(string relativeUri, CancellationToken cancellationToken = default);
    Task AddAsync(string relativeUri, Stream content, bool @override = false, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(string relativeUri, CancellationToken cancellationToken = default);
    Task<Stream> GetAsync(string relativeUri, CancellationToken cancellationToken = default);
    Task<IEnumerable<BlobMetadata>> GetListAsync(string relativeUri, CancellationToken cancellationToken = default);
    Task CopyAsync(string relativeUri, Uri absoluteUri, CancellationToken cancellationToken);
}
