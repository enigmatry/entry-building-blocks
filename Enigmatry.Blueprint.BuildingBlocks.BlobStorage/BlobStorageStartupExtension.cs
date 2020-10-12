using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Enigmatry.Blueprint.BuildingBlocks.Azure.BlobStorage;
using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Enigmatry.Blueprint.BuildingBlocks.BlobStorage
{
    public static class BlobStorageStartupExtension
    {
        public static void AppAddBlobStorage(this IServiceCollection services, IConfiguration configuration, string containerName)
        {
            services.Configure<AzureBlobStorageSettings>(configuration.GetSection(AzureBlobStorageSettings.AppAzureBlobStorage));

            services.AddScoped<IBlobStorage>((serviceProvider) =>
            {
                var settings = serviceProvider.GetService<IOptionsSnapshot<AzureBlobStorageSettings>>();
                var service = new BlobServiceClient(settings.Value.ConnectionString);
                var container = service.GetBlobContainerClient(containerName);
                return new AzureBlobStorage(
                    container.Exists() ? container : service.CreateBlobContainer(containerName, PublicAccessType.Blob), settings);
            });

            services.AddScoped<IPrivateBlobStorage>((serviceProvider) =>
            {
                var settings = serviceProvider.GetService<IOptionsSnapshot<AzureBlobStorageSettings>>();
                var service = new BlobServiceClient(settings.Value.ConnectionString);
                var container = service.GetBlobContainerClient(containerName);
                return new AzurePrivateBlobStorage(
                    container.Exists() ? container : service.CreateBlobContainer(containerName, PublicAccessType.Blob), settings);
            });
        }
    }
}
