using Microsoft.Extensions.Options;

namespace Enigmatry.Entry.TemplatingEngine.Liquid.ValueConverters;

public class DateTimeValueConverter(IOptions<FluidTemplateEngineOptions> options) : IFluidValueConverter
{
    private readonly FluidTemplateEngineOptions _options = options.Value;

    public object? Convert(object? value)
    {
        if (value is not DateTime dateTime)
        {
            return null;
        }

        return TimeZoneInfo.ConvertTime(dateTime, _options.TimeZoneInfo).ToString(_options.CultureInfo);
    }
}
