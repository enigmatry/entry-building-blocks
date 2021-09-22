using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.BuildingBlocks.BlobStorage.Azure
{
    public static class AzureBlobStorageServiceCollectionExtension
    {
        public const string ConfigurationKey = "App:AzureBlobStorage";

        /// <summary>
        /// Registers an implementation of <see cref="IBlobStorage"/> and ensures
        /// that the blob container with the given <paramref name="name"/> is created.
        /// The implementation uses <see cref="PublicAccessType.Blob"/> access type for the blob container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="name">Azure Blob Storage container name.</param>
        public static IServiceCollection AppAddPublicAzBlobStorage(this IServiceCollection services, string name) =>
            services.AddSingleton<IBlobStorage>(provider =>
            {
                var settings = ResolveSettings(provider);
                var container = CreateContainer(name, PublicAccessType.Blob, settings);
                return new AzureBlobStorage(container, settings);
            });

        /// <summary>
        /// Registers an implementation of <see cref="IPrivateBlobStorage"/> and ensures
        /// that the blob container with the given <paramref name="name"/> is created.
        /// The implementation uses <see cref="PublicAccessType.None"/> access type for the blob container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="name">Azure Blob Storage container name.</param>
        public static IServiceCollection AppAddPrivateAzBlobStorage(this IServiceCollection services, string name) =>
            services.AddSingleton<IPrivateBlobStorage>(provider =>
            {
                var settings = ResolveSettings(provider);
                var container = CreateContainer(name, PublicAccessType.None, settings);
                return new AzurePrivateBlobStorage(container, settings);
            });

        private static BlobContainerClient CreateContainer(string name, PublicAccessType access,
            AzureBlobStorageSettings settings)
        {
            var service = new BlobServiceClient(settings.ConnectionString);
            var container = service.GetBlobContainerClient(name);
            return !container.Exists() ? service.CreateBlobContainer(name, access).Value : container;
        }

        private static AzureBlobStorageSettings ResolveSettings(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            return configuration.GetValue<AzureBlobStorageSettings>(ConfigurationKey);
        }
    }
}
