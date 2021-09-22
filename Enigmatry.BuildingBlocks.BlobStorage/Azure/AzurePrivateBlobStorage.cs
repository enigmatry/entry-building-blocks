using System;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Enigmatry.BuildingBlocks.BlobStorage.Azure
{
    internal class AzurePrivateBlobStorage : AzureBlobStorage, IPrivateBlobStorage
    {
        public AzurePrivateBlobStorage(BlobContainerClient container, AzureBlobStorageSettings settings)
        : base(container, settings) { }

        public string BuildSharedResourcePath(string path, PrivateBlobPermission permission = PrivateBlobPermission.Read)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            var uri = new Uri(BuildResourcePath(path));
            var query = BuildSasQueryParams(path, permission);
            var builder = new UriBuilder(uri) { Query = query.ToString() };
            return builder.ToString();
        }

        private BlobSasQueryParameters BuildSasQueryParams(string path, PrivateBlobPermission permission)
        {
            var builder = new BlobSasBuilder
            {
                StartsOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddSeconds(Settings.SasDuration),
                BlobContainerName = Container.Name,
                BlobName = path,
                Protocol = SasProtocol.Https
            };

            builder.SetPermissions(permission.ToBlobSasPermissions());

            var credential = new StorageSharedKeyCredential(Settings.AccountName, Settings.AccountKey);
            return builder.ToSasQueryParameters(credential);
        }
    }
}
