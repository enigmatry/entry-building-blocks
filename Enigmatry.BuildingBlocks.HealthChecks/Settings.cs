using System;
using Enigmatry.BuildingBlocks.Core.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Enigmatry.BuildingBlocks.HealthChecks
{
    [ExcludeFromCodeCoverage]
    internal class Settings
    {
        internal const string SectionName = "HealthChecks";

        internal int MaximumAllowedMemoryInMegaBytes { get; set; }
        internal string RequiredToken { get; set; } = String.Empty;

        internal bool TokenAuthorizationEnabled => RequiredToken.HasContent();
    }
}
