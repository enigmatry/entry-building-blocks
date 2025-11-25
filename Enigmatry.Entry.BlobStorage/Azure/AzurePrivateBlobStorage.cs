using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Enigmatry.Entry.BlobStorage.Models;
using Enigmatry.Entry.Core.Helpers;

namespace Enigmatry.Entry.BlobStorage.Azure;

internal class AzurePrivateBlobStorage : AzureBlobStorage, IPrivateBlobStorage
{
    public AzurePrivateBlobStorage(BlobContainerClient container, AzureBlobStorageSettings settings)
        : base(container, settings)
    {
    }

    public bool VerifySharedResourcePath(Uri uri)
    {
        if (!AzureBlobSharedUri.TryParse(uri, out var sasUri))
        {
            return false;
        }

        var validSignature = BuildSasQueryParams(sasUri.BlobName, sasUri.GetResponseHeaders(), sasUri.Permission, sasUri.ExpiresOn).Signature;
        return sasUri.Signature == validSignature;
    }

    public string BuildSharedResourcePath(string relativePath,
        PrivateBlobPermission permission = PrivateBlobPermission.Read,
        BlobResponseHeadersOverride? responseHeaders = null)
    {
        if (relativePath.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        DateTimeOffset expiresOn = DateTime.UtcNow.Add(Settings.SasDuration);
        var uri = new Uri(BuildResourcePath(relativePath));
        var query = BuildSasQueryParams(relativePath, responseHeaders, permission, expiresOn);
        var builder = new UriBuilder(uri) { Query = query.ToString() };
        return builder.ToString();
    }

    private BlobSasQueryParameters BuildSasQueryParams(string blob,
        BlobResponseHeadersOverride? responseHeaders,
        PrivateBlobPermission permission,
        DateTimeOffset expiresOn)
    {
        var builder = new BlobSasBuilder
        {
            ExpiresOn = expiresOn,
            BlobContainerName = Container.Name,
            BlobName = blob,
            Protocol = SasProtocol.Https,
            CacheControl = responseHeaders?.CacheControl.ToEmptyIfNull(),
            ContentEncoding = responseHeaders?.ContentEncoding.ToEmptyIfNull(),
            ContentLanguage = responseHeaders?.ContentLanguage.ToEmptyIfNull(),
            ContentType = responseHeaders?.ContentType.ToEmptyIfNull()
        };

        var contentDisposition = responseHeaders?.ContentDisposition?.ToString();
        if (contentDisposition.HasContent())
        {
            builder.ContentDisposition = contentDisposition;
        }
        builder.SetPermissions(permission.ToBlobSasPermissions());

        var credential = new StorageSharedKeyCredential(Settings.AccountName, Settings.AccountKey);
        return builder.ToSasQueryParameters(credential);
    }
}
