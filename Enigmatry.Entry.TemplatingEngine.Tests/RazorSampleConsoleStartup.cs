using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Enigmatry.Entry.TemplatingEngine.Tests;

// example startup class - show how to initialize Razor in console application
// and RazorTemplatingEngine class. This class can be used for email templating purposes.
public class RazorSampleConsoleStartup
{
    private readonly IHostEnvironment _environment;

    public RazorSampleConsoleStartup(IHostEnvironment environment)
    {
        _environment = environment;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages().AddRazorRuntimeCompilation(options =>
        {
            options.FileProviders.Clear();
            options.FileProviders.Add(new PhysicalFileProvider(_environment.ContentRootPath));
        });

        services.AppAddTemplatingEngine();
    }

    [UsedImplicitly]
#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable IDE0060 // Remove unused parameter
    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1801 // Review unused parameters
    {
    }
}
