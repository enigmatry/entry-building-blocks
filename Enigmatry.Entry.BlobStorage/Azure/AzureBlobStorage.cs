﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.Entry.BlobStorage.Azure;

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

    public string BuildResourcePath(string relativePath) =>
        !string.IsNullOrWhiteSpace(relativePath)
            ? new UriBuilder { Scheme = Container.Uri.Scheme, Host = Container.Uri.Host, Path = Path.Combine(Container.Name, relativePath) }.ToString()
            : relativePath;

    public async Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken = default) =>
        await Container.GetBlobClient(relativePath).ExistsAsync(cancellationToken);

    public async Task CopyAsync(string relativePath, Uri absoluteUri, CancellationToken cancellationToken) =>
        await Container.GetBlobClient(relativePath)
            .StartCopyFromUriAsync(absoluteUri, cancellationToken: cancellationToken);

    public async Task AddAsync(string relativePath, Stream content, bool @override = false, CancellationToken cancellationToken = default)
    {
        var blob = Container.GetBlobClient(relativePath);
        await blob.UploadAsync(content, @override, cancellationToken);

        var headers = ConfigureBlobHttpHeadersAsync(blob, cancellationToken);
        await blob.SetHttpHeadersAsync(headers, cancellationToken: cancellationToken);
    }

    public async Task<bool> RemoveAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        if (!relativePath.Contains('*', StringComparison.OrdinalIgnoreCase))
        {
            return await Container.DeleteBlobIfExistsAsync(relativePath, cancellationToken: cancellationToken);
        }

        await foreach (var blob in Container.GetBlobsAsync(
                           prefix: relativePath.Replace('\\', '/')
                               .Remove(relativePath.IndexOf('*', StringComparison.OrdinalIgnoreCase)),
                           cancellationToken: cancellationToken))
        {
            await Container.GetBlobClient(blob.Name).DeleteAsync(cancellationToken: cancellationToken);
        }

        return true;
    }

    public async Task<Stream> GetAsync(string relativePath, CancellationToken cancellationToken = default) =>
        (await Container.GetBlobClient(relativePath).DownloadAsync(cancellationToken)).Value.Content;

    internal virtual BlobHttpHeaders ConfigureBlobHttpHeadersAsync(BlobClient blob, CancellationToken cancellationToken = default)
    {
        var headers = new BlobHttpHeaders
        {
#pragma warning disable CA1308
            ContentType = Path.GetExtension(blob.Name).ToLower(CultureInfo.InvariantCulture) switch
#pragma warning restore CA1308
            {
                ".pdf" => "application/pdf",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            }
        };
        if (Settings.CacheTimeout > 0)
        {
            headers.CacheControl = $"public, max-age={Settings.CacheTimeout}";
        }

        return headers;
    }
}
