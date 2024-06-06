using Enigmatry.Entry.TemplatingEngine.Liquid.Filters;
using Enigmatry.Entry.TemplatingEngine.Liquid.ValueConverters;
using Fluid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public class FluidTemplatingEngine(
    IFluidTemplateProvider templateProvider,
    IOptionsSnapshot<FluidTemplateEngineOptions> options,
    IEnumerable<IFluidValueConverter> valueConverters,
    IEnumerable<IFluidFilter> customFilters,
    ILogger<FluidTemplatingEngine> logger)
    : ITemplatingEngine
{
    private readonly FluidTemplateEngineOptions _options = options.Value;

    public async Task<string> RenderAsync<T>(string pattern, T model)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentNullException.ThrowIfNull(model);

        var fluidTemplate = templateProvider.GetTemplate(pattern);

        var options = new TemplateOptions
        {
            MemberAccessStrategy =
                new LoggingUnsafeMemberAccessStrategy(logger) { MemberNameStrategy = _options.MemberNameStrategy },
            CultureInfo = _options.CultureInfo
        };

        // Reversing the order of the value converters to ensure that custom registered ones are executed before the default (entry) ones
        options.ValueConverters.AddRange(
            valueConverters.Reverse().Select<IFluidValueConverter, Func<object?, object?>>(converter => converter.Convert));

        foreach (var filter in customFilters)
        {
            options.Filters.AddFilter(filter.FilterName, filter.Filter);
        }

        var context = new TemplateContext(model, options) { TimeZone = _options.TimeZoneInfo };
        return await fluidTemplate.RenderAsync(context);
    }
}
