﻿using Enigmatry.Entry.AspNetCore.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;
using Enigmatry.Entry.HealthChecks.Extensions;
using Enigmatry.Entry.Swagger;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp;

[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable",
    Justification = "This rule doesn't apply for Program.cs")]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var mvcBuilder = builder.Services.AddControllers();

        // authorization building block targets only NET7 
#if NET7_0_OR_GREATER
        builder.Services.AppAddAuthorization();
#endif

        builder.Services.AppAddSwagger("SampleApp");
        builder.Services.AppAddHealthChecks(builder.Configuration);

        ConfigureMvc(mvcBuilder, SampleAppSettings.Default());

        var app = builder.Build();

        app.AppUseExceptionHandler();

        app.MapControllers().RequireAuthorization();
        app.AppMapHealthCheck(app.Configuration);

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.AppUseSwagger();

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
