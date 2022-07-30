﻿using Enigmatry.BuildingBlocks.HealthChecks.Authorization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.HealthChecks.Extensions
{
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static class StartupExtensions
    {
        public static void AppMapHealthCheck(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
        {
            var healthCheckSettings = configuration.ResolveHealthCheckSettings();
            var healthCheckEndpoint = endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions
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

            if (healthCheckSettings.TokenAuthorizationEnabled)
            {
                healthCheckEndpoint.RequireAuthorization(TokenRequirement.Name);
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
