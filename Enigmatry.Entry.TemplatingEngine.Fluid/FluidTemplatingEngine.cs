using Enigmatry.Entry.Core.Helpers;
using Enigmatry.Entry.TemplatingEngine.Liquid.CustomFilters;
using Fluid;
using Fluid.Values;
using Microsoft.Extensions.Logging;
using System.Globalization;

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

    public async Task<string> RenderAsync<T>(string path, T model)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(model);

        var fluidTemplate = _templateProvider.GetTemplate(path);
        var options = new TemplateOptions
        {
            MemberAccessStrategy =
                new LoggingUnsafeMemberAccessStrategy(_logger) { MemberNameStrategy = _options.MemberNameStrategy },
            CultureInfo = _options.CultureInfo
        };

        if (_options.ConvertEnumToString)
        {
            options.ValueConverters.Add(value => value is Enum e ? new StringValue(e.GetDescription()) : null);
        }

        options.ValueConverters.Add(value => value is DateTimeOffset dateTime
            ? dateTime.ToDutchDateTime().ToString("dd-MM-yyyy HH:mm:ss", _options.CultureInfo)
            : null);

        foreach (var filter in _fluidFilters)
        {
            options.Filters.AddFilter(filter.FilterName, filter.Filter);
        }

        var context = new TemplateContext(model, options) { TimeZone = DateTimeOffsetExtensions.WestEuropeZone };
        return await fluidTemplate.RenderAsync(context);
    }

    public async Task<string> RenderAsync<T>(string path, T model, IDictionary<string, object> viewBagDictionary) =>
        await RenderAsync(path, model);
}
