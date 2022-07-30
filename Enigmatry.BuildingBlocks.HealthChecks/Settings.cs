using Enigmatry.BuildingBlocks.Core.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Enigmatry.BuildingBlocks.HealthChecks
{
    [ExcludeFromCodeCoverage]
    public class Settings
    {
        public const string SectionName = "HealthChecks";

        public int MaximumAllowedMemoryInMegaBytes { get; set; }
        public string RequiredToken { get; set; } = string.Empty;

        public bool TokenAuthorizationEnabled => RequiredToken.HasContent();
    }
}
