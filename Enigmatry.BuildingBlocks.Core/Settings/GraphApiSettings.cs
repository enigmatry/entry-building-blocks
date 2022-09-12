using System;

namespace Enigmatry.BuildingBlocks.Core.Settings
{
    public class GraphApiSettings
    {
        public const string GraphApiSectionName = "App:GraphApi";

        public bool Enabled { get; set; }
        public string TenantId { get; set; } = String.Empty;
        public string ClientId { get; set; } = String.Empty;
        public string ClientSecret { get; set; } = String.Empty;
        public string PasswordPolicies { get; set; } = "DisablePasswordExpiration";
    }
}
