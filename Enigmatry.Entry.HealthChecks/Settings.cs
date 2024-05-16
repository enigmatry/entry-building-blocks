using Enigmatry.Entry.Core.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Enigmatry.Entry.HealthChecks;

[ExcludeFromCodeCoverage]
internal class Settings
{
    internal const string SectionName = "HealthChecks";

    public int MaximumAllowedMemoryInMegaBytes { get; set; }
    public string RequiredToken { get; set; } = string.Empty;

    internal bool TokenAuthorizationEnabled => RequiredToken.HasContent();
}
