using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Enigmatry.BuildingBlocks.BlobStorage.Azure
{
    internal class AzureBlobStorage : IBlobStorage
    {
        protected AzureBlobStorageSettings Settings { get; }
        protected BlobContainerClient Container { get; }

        public AzureBlobStorage(BlobContainerClient container, AzureBlobStorageSettings options)
        {
            Settings = options;
            Container = container;
            Name = container.Name;
        }

        public string Name { get; }

        public string BuildResourcePath(string path) =>
            !String.IsNullOrWhiteSpace(path)
                ? new UriBuilder { Scheme = Container.Uri.Scheme, Host = Container.Uri.Host, Path = Path.Combine(Container.Name, path) }.ToString()
                : path;

        public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default) =>
            await Container.GetBlobClient(path).ExistsAsync(cancellationToken);

        public async Task CopyAsync(string path, Uri from, CancellationToken cancellationToken) =>
            await Container.GetBlobClient(path)
                .StartCopyFromUriAsync(from, cancellationToken: cancellationToken);

        public async Task AddAsync(string path, Stream content, bool @override = false, CancellationToken cancellationToken = default)
        {
            var blob = Container.GetBlobClient(path);
            await blob.UploadAsync(content, @override, cancellationToken);

            var headers = ConfigureBlobHttpHeadersAsync(blob, cancellationToken);
            await blob.SetHttpHeadersAsync(headers, cancellationToken: cancellationToken);
        }

        public async Task<bool> RemoveAsync(string path, CancellationToken cancellationToken = default)
        {
            if (!path.Contains('*'))
                return await Container.DeleteBlobIfExistsAsync(path, cancellationToken: cancellationToken);

            await foreach (var blob in Container.GetBlobsAsync(prefix: path.Replace('\\', '/').Remove(path.IndexOf('*'))))
            {
                await Container.GetBlobClient(blob.Name).DeleteAsync(cancellationToken: cancellationToken);
            }

            return true;
        }

        public async Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default) =>
            (await Container.GetBlobClient(path).DownloadAsync(cancellationToken)).Value.Content;

        internal virtual BlobHttpHeaders ConfigureBlobHttpHeadersAsync(BlobClient blob, CancellationToken cancellationToken = default)
        {
            var headers = new BlobHttpHeaders
            {
                ContentType = Path.GetExtension(blob.Name).ToLower() switch
                {
                    ".pdf" => "application/pdf",
                    ".svg" => "image/svg+xml",
                    _ => "application/octet-stream"
                }
            };
            if (Settings.CacheTimeout > 0)
                headers.CacheControl = $"public, max-age={Settings.CacheTimeout}";

            return headers;
        }
    }
}
