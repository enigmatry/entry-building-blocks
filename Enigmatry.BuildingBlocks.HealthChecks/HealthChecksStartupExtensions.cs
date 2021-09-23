using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Enigmatry.BuildingBlocks.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Enigmatry.BuildingBlocks.HealthChecks
{
    public static class HealthChecksStartupExtensions
    {
        public static void AppMapHealthCheck(this IEndpointRouteBuilder endpoints, HealthCheckSettings settings)
        {
            var healthCheckEndpoint = endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions()
            {
                // Specify a custom ResponseWriter, so we can return json with additional information,
                // Otherwise it will just return plain text with the status.
                ResponseWriter = WriteResponse,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });

            if (settings.TokenAuthorizationEnabled)
            {
                healthCheckEndpoint.RequireAuthorization(HealthChecksTokenRequirement.Name);
            }
        }

        public static void AppAddHealthChecks<T>(this IServiceCollection services, IConfiguration configuration, Func<IServiceCollection, T, IHealthChecksBuilder> buildHealthChecks)
            where T : HealthCheckSettings
        {
            var healthCheckSettings = configuration.GetSection(HealthCheckSettings.HealthChecksSectionName).Get<T>();
            var appInsightsSettings = configuration.GetSection(ApplicationInsightsSettings.ApplicationInsightsSectionName).Get<ApplicationInsightsSettings>();

            // Here we can configure the different health checks:
            var healthChecks = buildHealthChecks(services, healthCheckSettings);
            healthChecks.AddPrivateMemoryHealthCheck(1024 * 1024 * healthCheckSettings.MaximumAllowedMemoryInMegaBytes, "Available memory test", HealthStatus.Degraded);

            // We can also push the results to Application Insights. This will be done every 30 seconds
            // Can be checked from the Azure Portal under metrics, by selecting the azure.applicationinsights namespace.
            if (!String.IsNullOrEmpty(appInsightsSettings.InstrumentationKey))
            {
                healthChecks.AddApplicationInsightsPublisher(appInsightsSettings.InstrumentationKey, true);
            }

            if (healthCheckSettings.TokenAuthorizationEnabled)
            {
                services.AddAuthorization(options => options.AddPolicy(HealthChecksTokenRequirement.Name, policy => policy.Requirements.Add(new HealthChecksTokenRequirement(healthCheckSettings.RequiredToken))));
                services.AddSingleton<IAuthorizationHandler, HealthChecksTokenHandler>();
            }
        }

        private static async Task WriteResponse(HttpContext context, HealthReport report) =>
            await context.Response.WriteAsJsonAsync(
                new
                {
                    status = report.Status.ToString(),
                    entries = report.Entries.Select(keyValuePair =>
                        new { key = keyValuePair.Key, value = keyValuePair.Value.Status.ToString() })
                }, new JsonSerializerOptions { WriteIndented = true });
    }
}
