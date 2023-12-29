using System;
using Enigmatry.Entry.BlobStorage.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.BlobStorage
{
    public static class BlobStorageStartupExtension
    {
        [Obsolete("The method is deprecated and will be removed in future releases. " +
                  "Try using AddEntryPublicAzBlobStorage/AddEntryPrivateAzBlobStorage methods from Enigmatry.Entry.BlobStorage.Azure")]
        public static void AddEntryBlobStorage(this IServiceCollection services, IConfiguration _, string containerName)
        {
            services.AddEntryPublicAzBlobStorage(containerName);
            services.AddEntryPrivateAzBlobStorage(containerName);
        }
    }
}
