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
            // If you set the start time for a SAS to the current time, failures might occur intermittently for the first few minutes.
            // This is due to different machines having slightly different current times (known as clock skew).
            // In general, set the start time to be at least 15 minutes in the past.  Or, don't set it at all,
            // which will make it valid immediately in all cases. The same generally applies to expiry time as well - remember that
            // you may observe up to 15 minutes of clock skew in either direction on any request.
            var builder = new BlobSasBuilder
            {
                ExpiresOn = DateTime.UtcNow.Add(Settings.SasDuration),
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
