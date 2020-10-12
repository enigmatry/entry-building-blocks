using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Enigmatry.Blueprint.BuildingBlocks.BlobStorage;
using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
using Microsoft.Extensions.Options;
using System;

namespace Enigmatry.Blueprint.BuildingBlocks.Azure.BlobStorage
{
    internal class AzurePrivateBlobStorage : AzureBlobStorage, IPrivateBlobStorage
    {
        public AzurePrivateBlobStorage(BlobContainerClient container, IOptions<AzureBlobStorageSettings> settings)
        : base(container, settings) { }

        public string BuildSharedResourcePath(string path)
        {
            if (String.IsNullOrWhiteSpace(path)) return path;

            var sasBuilder = new BlobSasBuilder
            {
                StartsOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddSeconds(Settings.SasDuration),
                BlobContainerName = Container.Name,
                BlobName = path,
                Protocol = SasProtocol.Https
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return new UriBuilder(new Uri(BuildResourcePath(path)))
            {
                Query = sasBuilder
                    .ToSasQueryParameters(new StorageSharedKeyCredential(Settings.AccountName, Settings.AccountKey))
                    .ToString()
            }
            .ToString();
        }
    }
}
