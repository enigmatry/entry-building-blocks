namespace Enigmatry.Entry.BlobStorage;

public interface IBlobStorage
{
    public string Name { get; }
    public string BuildResourcePath(string relativeUri);
    public Task<bool> ExistsAsync(string relativeUri, CancellationToken cancellationToken = default);
    public Task AddAsync(string relativeUri, Stream content, bool @override = false, CancellationToken cancellationToken = default);
    public Task<bool> RemoveAsync(string relativeUri, CancellationToken cancellationToken = default);
    public Task<Stream> GetAsync(string relativeUri, CancellationToken cancellationToken = default);
    public Task CopyAsync(string relativeUri, Uri absoluteUri, CancellationToken cancellationToken);
}
