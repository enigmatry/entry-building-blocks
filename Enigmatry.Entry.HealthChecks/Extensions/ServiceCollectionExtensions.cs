using Enigmatry.Entry.HealthChecks.Authorization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace Enigmatry.Entry.HealthChecks.Extensions;

public static class ServiceCollectionExtensions
{
    [PublicAPI]
    public static IHealthChecksBuilder AppAddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var settings = configuration.ResolveHealthCheckSettings();
        var healthChecksBuilder = services.AddHealthChecks();

        if (settings.TokenAuthorizationEnabled)
        {
            InitializeTokenAuthorization(services, settings);
        }

        Initialize(healthChecksBuilder, settings);
        return healthChecksBuilder;
    }

    private static void Initialize(IHealthChecksBuilder healthChecksBuilder, Settings settings)
    {
        const int megabyte = 1024 * 1024;
        healthChecksBuilder.AddPrivateMemoryHealthCheck(megabyte * settings.MaximumAllowedMemoryInMegaBytes,
            "Available memory test", HealthStatus.Degraded);
    }

    private static void InitializeTokenAuthorization(IServiceCollection services, Settings settings)
    {
        services.AddAuthorization(options => options.AddPolicy(TokenRequirement.Name,
            policy => policy.Requirements.Add(new TokenRequirement(settings.RequiredToken))));
        services.AddSingleton<IAuthorizationHandler, TokenHandler>();
    }
}
