using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.TemplatingEngine
{
    public static class TemplatingEngineStartupExtensions
    {
        public static void AddEntryTemplatingEngine(this IServiceCollection services) =>
            services.AddScoped<ITemplatingEngine, RazorTemplatingEngine>();
    }
}
