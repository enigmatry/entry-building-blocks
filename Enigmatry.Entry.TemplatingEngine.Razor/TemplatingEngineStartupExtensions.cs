using Enigmatry.Entry.Core.Templating;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.TemplatingEngine
{
    public static class TemplatingEngineStartupExtensions
    {
        [Obsolete("Use AddEntryTemplatingEngine instead")]
        public static void AppAddTemplatingEngine(this IServiceCollection services) => services.AddEntryTemplatingEngine();

        public static void AddEntryTemplatingEngine(this IServiceCollection services) =>
            services.AddScoped<ITemplatingEngine, RazorTemplatingEngine>();
    }
}
