using Enigmatry.Entry.AspNetCore.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp.Controllers;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp.Startup;
using Enigmatry.Entry.HealthChecks.Extensions;
using Enigmatry.Entry.Swagger;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp;

[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable",
    Justification = "This rule doesn't apply for Program.cs")]
public class Program
{
    private static readonly Assembly ThisAssembly = typeof(UpdateWeatherForecast.Request).Assembly;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AppAddMediatR(ThisAssembly);
        builder.Services.AppAddValidation(ThisAssembly);

        var mvcBuilder = builder.Services.AddControllers();

        builder.Services.AppAddAuthorization();
        builder.Services.AddEntrySwagger("SampleApp");
        builder.Services.AddEntryHealthChecks(builder.Configuration);

        ConfigureMvc(mvcBuilder, SampleAppSettings.Default());

        var app = builder.Build();

        app.UseEntryExceptionHandler();

        app.MapControllers().RequireAuthorization();
        app.MapEntryHealthCheck(app.Configuration);

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEntrySwagger();

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
