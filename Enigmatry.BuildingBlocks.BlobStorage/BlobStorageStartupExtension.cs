using System;
using Enigmatry.BuildingBlocks.BlobStorage.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.BuildingBlocks.BlobStorage
{
    public static class BlobStorageStartupExtension
    {
        [Obsolete("The method is deprecated and will be removed in future releases. " +
                  "Try using AppAddPublicBlobStorage/AppAddPrivateBlobStorage methods from Enigmatry.BuildingBlocks.BlobStorage.Azure")]
        public static void AppAddBlobStorage(this IServiceCollection services, IConfiguration _, string containerName)
        {
            services.AppAddPublicAzBlobStorage(containerName);
            services.AppAddPrivateAzBlobStorage(containerName);
        }
    }
}
