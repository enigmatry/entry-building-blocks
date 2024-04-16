using Fluid;
using System.Globalization;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public class FluidTemplateEngineOptions
{
    public bool ConvertEnumToString { get; set; } = true;
    public MemberNameStrategy MemberNameStrategy { get; set; } = MemberNameStrategies.CamelCase;
    public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;
}
