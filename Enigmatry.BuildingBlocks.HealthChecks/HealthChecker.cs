using Enigmatry.BuildingBlocks.HealthChecks.Authorization;
using Enigmatry.BuildingBlocks.HealthChecks.Extensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Enigmatry.BuildingBlocks.HealthChecks
{
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public class HealthChecker
    {
        private readonly Settings _settings;
        private readonly IHealthChecksBuilder _healthChecksBuilder;

        public HealthChecker(IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _settings = configuration.ResolveHealthCheckSettings();
            _healthChecksBuilder = services.AddHealthChecks();

            if (_settings.TokenAuthorizationEnabled)
            {
                InitializeTokenAuthorization(services);
            }

            InitializeHealthCheckBuilder();
        }

        /// <summary>
        /// We can also push the results to Application Insights. This will be done every 30 seconds.
        /// Can be checked from the Azure Portal under metrics, by selecting the azure.applicationinsights namespace.
        /// </summary>
        /// <param name="instrumentationKey">Application Insights instrumentation key.</param>
        /// <returns>This checker.</returns>
        /// <exception cref="ArgumentException">When key is not passed.</exception>
        public HealthChecker ConfigureApplicationInsights(string instrumentationKey)
        {
            if (string.IsNullOrEmpty(instrumentationKey))
            {
                throw new ArgumentException("Valid instrumentation key has to be provided!", nameof(instrumentationKey));
            }

            _healthChecksBuilder.AddApplicationInsightsPublisher(instrumentationKey, true);
            return this;
        }

        /// <summary>
        /// We can also monitor Sql Server database (context).
        /// </summary>
        /// <typeparam name="TContext">Desired DbContext to be monitored.</typeparam>
        /// <param name="connectionString">Database connection string.</param>
        /// <returns>This checker.</returns>
        /// <exception cref="ArgumentException">When connection string is not passed.</exception>
        public HealthChecker CheckSqlServerContext<TContext>(string connectionString) where TContext : DbContext
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Valid Sql Server connectionString has to be provided!", nameof(connectionString));
            }

            _healthChecksBuilder.AddSqlServer(connectionString, "SELECT NULL").AddDbContextCheck<TContext>();
            return this;
        }

        /// <summary>
        /// We can also monitor Azure Service Bus Topic.
        /// </summary>
        /// <param name="connectionString">Azure Service Bus topic connection string.</param>
        /// <param name="name">Azure Service Bus topic name.</param>
        /// <returns>This checker.</returns>
        /// <exception cref="ArgumentException">When connection string and/or name is not passed.</exception>
        public HealthChecker CheckAzureTopic(string connectionString, string name)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Valid Azure topic connectionString has to be provided!", nameof(connectionString));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Valid Azure topic name has to be provided!", nameof(name));
            }

            _healthChecksBuilder.AddAzureServiceBusTopic(connectionString, name);
            return this;
        }

        private void InitializeHealthCheckBuilder()
        {
            const int megabyte = 1024 * 1024;
            _healthChecksBuilder.AddPrivateMemoryHealthCheck(megabyte * _settings.MaximumAllowedMemoryInMegaBytes,
                "Available memory test", HealthStatus.Degraded);
        }

        private void InitializeTokenAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options => options.AddPolicy(TokenRequirement.Name,
                policy => policy.Requirements.Add(new TokenRequirement(_settings.RequiredToken))));
            services.AddSingleton<IAuthorizationHandler, TokenHandler>();
        }
    }
}
