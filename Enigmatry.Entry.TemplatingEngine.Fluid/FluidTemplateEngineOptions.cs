using Fluid;
using System.Globalization;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public class FluidTemplateEngineOptions
{
    public MemberNameStrategy MemberNameStrategy { get; set; } = MemberNameStrategies.CamelCase;
    public CultureInfo CultureInfo { get; set; } = CultureInfo.CurrentCulture;
    public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.Local;
}
