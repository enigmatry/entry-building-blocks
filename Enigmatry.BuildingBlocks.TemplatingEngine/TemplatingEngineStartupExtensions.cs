using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.BuildingBlocks.TemplatingEngine
{
    public static class TemplatingEngineStartupExtensions
    {
        public static void AppAddTemplatingEngine(this IServiceCollection services) =>
            services.AddScoped<ITemplatingEngine, RazorTemplatingEngine>();
    }
}
