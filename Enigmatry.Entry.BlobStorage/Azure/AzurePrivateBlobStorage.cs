using System.Text.RegularExpressions;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
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

        var validSignature = BuildSasQueryParams(sasUri.BlobName, sasUri.FileName, sasUri.Permission, sasUri.ExpiresOn).Signature;
        return sasUri.Signature == validSignature;
    }


    public string BuildSharedResourcePath(string relativePath,
        string? fileName = null,
        PrivateBlobPermission permission = PrivateBlobPermission.Read)
    {
        DateTimeOffset expiresOn = DateTime.UtcNow.Add(Settings.SasDuration);
        if (relativePath.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        var uri = new Uri(BuildResourcePath(relativePath));
        var query = BuildSasQueryParams(relativePath, fileName, permission, expiresOn);
        var builder = new UriBuilder(uri) { Query = query.ToString() };
        return builder.ToString();
    }

    private BlobSasQueryParameters BuildSasQueryParams(string blob,
        string? fileName,
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

        if (fileName.HasContent())
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var pattern = $"[{string.Join(string.Empty, invalidChars.Select(c => Regex.Escape(c.ToString())))}]";
            var sanitizedFileName = Regex.Replace(fileName!, pattern, "_");
            builder.ContentDisposition = $"{AzureBlobSharedUri.ContentDispositionPrefix}{sanitizedFileName}";
        }
        builder.SetPermissions(permission.ToBlobSasPermissions());

        var credential = new StorageSharedKeyCredential(Settings.AccountName, Settings.AccountKey);
        return builder.ToSasQueryParameters(credential);
    }
}
