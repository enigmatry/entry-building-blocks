using System;
using Enigmatry.Entry.Core.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Enigmatry.Entry.HealthChecks
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
