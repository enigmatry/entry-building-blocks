using Microsoft.Extensions.Options;

namespace Enigmatry.Entry.TemplatingEngine.Liquid.ValueConverters;

public class DateTimeOffsetValueConverter(IOptionsSnapshot<FluidTemplateEngineOptions> options) : IFluidValueConverter
{
    private readonly FluidTemplateEngineOptions _options = options.Value;

    public object? Convert(object? value)
    {
        if (value is not DateTimeOffset inputDateTime)
        {
            return null;
        }

        var localOffset = _options.TimeZoneInfo.GetUtcOffset(inputDateTime);
        if (inputDateTime.Offset != localOffset)
        {
            inputDateTime = inputDateTime.ToOffset(localOffset);
        }

        return inputDateTime.DateTime.ToString(_options.CultureInfo);
    }
}
