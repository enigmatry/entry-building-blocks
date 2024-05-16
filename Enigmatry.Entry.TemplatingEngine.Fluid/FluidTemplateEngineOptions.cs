using Fluid;
using System.Globalization;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public class FluidTemplateEngineOptions
{
    public static readonly string SectionName = "FluidTemplateEngineOptions";
    public bool ConvertEnumToString { get; set; } = true;
    public MemberNameStrategy MemberNameStrategy { get; set; } = MemberNameStrategies.CamelCase;
    public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;
    public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard time");
    public string DateTimeFormat { get; set; } = "dd-MM-yyyy HH:mm:ss";
}
