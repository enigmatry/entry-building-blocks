using Enigmatry.Entry.AspNetCore.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp;

#pragma warning disable CA1052

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var mvcBuilder = builder.Services.AddControllers();

        ConfigureMvc(mvcBuilder, SampleAppSettings.Default());

        var app = builder.Build();

        app.AppUseExceptionHandler<Program>();

        app.MapControllers();

        app.Run();
    }

    internal static void ConfigureMvc(IMvcBuilder mvcBuilder, SampleAppSettings settings)
    {
        if (settings.UseNewtonsoftJson)
        {
            mvcBuilder.AddNewtonsoftJson();
        }
    }
}
