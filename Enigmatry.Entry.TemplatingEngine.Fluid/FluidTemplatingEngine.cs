using Enigmatry.Entry.TemplatingEngine.Liquid.CustomFilters;
using Fluid;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public class FluidTemplatingEngine : ITemplatingEngine
{
    private readonly ILogger<FluidTemplatingEngine> _logger;
    private readonly IFluidTemplateProvider _templateProvider;
    private readonly IEnumerable<ICustomFluidFilter> _fluidFilters;
    private readonly FluidTemplateEngineOptions _options;


    public FluidTemplatingEngine(ILogger<FluidTemplatingEngine> logger, IFluidTemplateProvider templateProvider,
        IEnumerable<ICustomFluidFilter> fluidFilters, FluidTemplateEngineOptions options)
    {
        _logger = logger;
        _templateProvider = templateProvider;
        _fluidFilters = fluidFilters;
        _options = options;
    }

    public async Task<string> RenderAsync<T>(string pattern, T model)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentNullException.ThrowIfNull(model);

        var fluidTemplate = _templateProvider.GetTemplate(pattern);
        var options = new TemplateOptions
        {
            MemberAccessStrategy =
                new LoggingUnsafeMemberAccessStrategy(_logger) { MemberNameStrategy = _options.MemberNameStrategy },
            CultureInfo = _options.CultureInfo
        };

        options.ValueConverters.AddRange(_options.ValueConverters);

        foreach (var filter in _fluidFilters)
        {
            options.Filters.AddFilter(filter.FilterName, filter.Filter);
        }

        var context = new TemplateContext(model, options) { TimeZone = _options.TimeZoneInfo };
        return await fluidTemplate.RenderAsync(context);
    }
}
