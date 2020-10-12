using System;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Settings
{
    public class AzureBlobStorageSettings
    {
        public const string AppAzureBlobStorage = "App:AzureBlobStorage";

        public string AccountKey { get; set; } = String.Empty;
        public string AccountName { get; set; } = String.Empty;
        public int SasDuration { get; set; }
        public string ConnectionString => $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey}";
        public long FileSizeLimit { get; set; }
        public int CacheTimeout { get; set; }
    }
}
