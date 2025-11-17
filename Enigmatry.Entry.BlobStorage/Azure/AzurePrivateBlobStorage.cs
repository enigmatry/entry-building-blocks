using System.Text.RegularExpressions;
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

        var validSignature = BuildSasQueryParams(sasUri.BlobName, sasUri.GetContentDisposition(), sasUri.Permission, sasUri.ExpiresOn).Signature;
        return sasUri.Signature == validSignature;
    }

    public string BuildSharedResourcePath(string relativePath,
        string fileName,
        PrivateBlobPermission permission = PrivateBlobPermission.Read)
    {
        var settings = fileName.HasContent()
            ? new ContentDisposition(fileName, ContentDispositionType.Attachment)
            : null;
        return BuildSharedResourcePath(relativePath, settings, permission);
    }

    public string BuildSharedResourcePath(string relativePath,
        ContentDisposition? settings = null,
        PrivateBlobPermission permission = PrivateBlobPermission.Read)
    {
        if (relativePath.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        DateTimeOffset expiresOn = DateTime.UtcNow.Add(Settings.SasDuration);
        var uri = new Uri(BuildResourcePath(relativePath));
        var query = BuildSasQueryParams(relativePath, settings, permission, expiresOn);
        var builder = new UriBuilder(uri) { Query = query.ToString() };
        return builder.ToString();
    }

    private BlobSasQueryParameters BuildSasQueryParams(string blob,
        ContentDisposition? settings,
        PrivateBlobPermission permission,
        DateTimeOffset expiresOn)
    {
        var builder = new BlobSasBuilder
        {
            ExpiresOn = expiresOn,
            BlobContainerName = Container.Name,
            BlobName = blob,
            Protocol = SasProtocol.Https
        };

        var contentDisposition = settings?.ToString();
        if (contentDisposition.HasContent())
        {
            builder.ContentDisposition = contentDisposition;
        }
        builder.SetPermissions(permission.ToBlobSasPermissions());

        var credential = new StorageSharedKeyCredential(Settings.AccountName, Settings.AccountKey);
        return builder.ToSasQueryParameters(credential);
    }
}
