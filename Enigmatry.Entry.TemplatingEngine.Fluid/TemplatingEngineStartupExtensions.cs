using Enigmatry.Entry.TemplatingEngine.Liquid.CustomFilters;
using Fluid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public static class TemplatingEngineStartupExtensions
{
    public static void AddLiquidTemplatingEngine(this IServiceCollection services)
    {
        services.AddSingleton<FluidParser>();
        services.AddScoped<IFluidTemplateProvider, FluidTemplateProvider>();
        services.AddScoped<ITemplatingEngine, FluidTemplatingEngine>(provider =>
            new FluidTemplatingEngine(provider.GetRequiredService<ILogger<FluidTemplatingEngine>>(),
                provider.GetRequiredService<IFluidTemplateProvider>(),
                provider.GetRequiredService<IEnumerable<ICustomFluidFilter>>(), provider.GetRequiredService<IOptions<FluidTemplateEngineOptions>>().Value));
    }
}
