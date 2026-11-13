using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.TemplatingEngine.Tests;

// example startup class - show how to initialize Razor in console application
// and RazorTemplatingEngine class. This class can be used for email templating purposes.
public class RazorSampleConsoleStartup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddEntryTemplatingEngine();
    }

    [UsedImplicitly]
#pragma warning disable IDE0060
    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore IDE0060
    {
    }
}
