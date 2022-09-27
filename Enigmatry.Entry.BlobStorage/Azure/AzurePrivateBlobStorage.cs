using System;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Enigmatry.Entry.BlobStorage.Azure
{
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

            var validSignature = BuildSasQueryParams(sasUri.BlobName, sasUri.Permission, sasUri.ExpiresOn).Signature;
            return sasUri.Signature == validSignature;
        }


        public string BuildSharedResourcePath(string relativePath,
            PrivateBlobPermission permission = PrivateBlobPermission.Read)
        {
            DateTimeOffset expiresOn = DateTime.UtcNow.Add(Settings.SasDuration);
            if (String.IsNullOrWhiteSpace(relativePath))
            {
                return String.Empty;
            }

            var uri = new Uri(BuildResourcePath(relativePath));
            var query = BuildSasQueryParams(relativePath, permission, expiresOn);
            var builder = new UriBuilder(uri) { Query = query.ToString() };
            return builder.ToString();
        }

        private BlobSasQueryParameters BuildSasQueryParams(string blob, PrivateBlobPermission permission, DateTimeOffset expiresOn)
        {
            var builder = new BlobSasBuilder
            {
                ExpiresOn = expiresOn,
                BlobContainerName = Container.Name,
                BlobName = blob,
                Protocol = SasProtocol.Https
            };
            builder.SetPermissions(permission.ToBlobSasPermissions());
            var credential = new StorageSharedKeyCredential(Settings.AccountName, Settings.AccountKey);
            return builder.ToSasQueryParameters(credential);
        }
    }
}
