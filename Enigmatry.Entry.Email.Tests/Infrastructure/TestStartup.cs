using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.Email.Tests.Infrastructure;

public class TestStartup
{
    private readonly IConfiguration _configuration;

    public TestStartup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [UsedImplicitly]
    public void ConfigureServices(IServiceCollection services) => services.AppAddEmailClient(_configuration);

    [UsedImplicitly]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Method signature is required, but has no implementation.")]
    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Nothing to do here.
    }
}
