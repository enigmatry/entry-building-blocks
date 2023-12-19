using System;

namespace Enigmatry.Entry.BlobStorage.Azure
{
    internal class AzureBlobStorageSettings
    {
        public string AccountKey { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public TimeSpan SasDuration { get; set; }
        public string ConnectionString => $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey}";
        public long FileSizeLimit { get; set; }
        public int CacheTimeout { get; set; }
    }
}
