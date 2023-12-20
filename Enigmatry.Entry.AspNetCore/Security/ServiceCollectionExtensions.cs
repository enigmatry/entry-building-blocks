using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;

namespace Enigmatry.Entry.AspNetCore.Security;

[PublicAPI]
public static class ServiceCollectionExtensions
{
    [Obsolete("Use AddEntryHttps instead")]
    public static void AppAddHttps(this IServiceCollection services, IHostEnvironment environment) => services.AddEntryHttps(environment);

    public static void AddEntryHttps(this IServiceCollection services, IHostEnvironment environment)
    {
        var isDevelopmentEnvironment = environment.IsDevelopment();

        if (!isDevelopmentEnvironment)
        {
            const int oneYear = 365;
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(oneYear);
            });
        }

        var redirection = isDevelopmentEnvironment
            ? HttpStatusCode.TemporaryRedirect
            : HttpStatusCode.PermanentRedirect;

        services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = (int)redirection;
        });
    }
}
