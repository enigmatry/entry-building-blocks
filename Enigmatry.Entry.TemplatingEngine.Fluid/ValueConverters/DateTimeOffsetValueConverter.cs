using Microsoft.Extensions.Options;

namespace Enigmatry.Entry.TemplatingEngine.Liquid.ValueConverters;

public class DateTimeOffsetValueConverter(IOptions<FluidTemplateEngineOptions> options) : IFluidValueConverter
{
    private readonly FluidTemplateEngineOptions _options = options.Value;

    public object? Convert(object? value)
    {
        if (value is not DateTimeOffset dateTimeOffset)
        {
            return null;
        }

        var localOffset = _options.TimeZoneInfo.GetUtcOffset(dateTimeOffset);
        if (dateTimeOffset.Offset != localOffset)
        {
            dateTimeOffset = dateTimeOffset.ToOffset(localOffset);
        }

        return dateTimeOffset.DateTime.ToString(_options.CultureInfo);
    }
}
