using Fluid;
using System.Globalization;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public class FluidTemplateEngineOptions
{
    public MemberNameStrategy MemberNameStrategy { get; init; } = MemberNameStrategies.CamelCase;
    public CultureInfo CultureInfo { get; init; } = CultureInfo.InvariantCulture;
    public TimeZoneInfo TimeZoneInfo { get; init; } = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard time");
    public IEnumerable<Func<object, object?>> ValueConverters { get; init; } = [];
}
