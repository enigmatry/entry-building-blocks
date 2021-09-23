using System;

namespace Enigmatry.BuildingBlocks.Core.Settings
{
    public class HealthCheckSettings
    {
        public const string HealthChecksSectionName = "HealthChecks";

        public int MaximumAllowedMemoryInMegaBytes { get; set; }
        public bool TokenAuthorizationEnabled { get; set; }
        public string RequiredToken { get; set; } = String.Empty;
    }
}
