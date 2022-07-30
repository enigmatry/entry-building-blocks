using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Enigmatry.BuildingBlocks.HealthChecks.Extensions
{
    [ExcludeFromCodeCoverage]
    internal static class ConfigurationExtensions
    {
        internal static Settings ResolveHealthCheckSettings(this IConfiguration configuration)
            => configuration.ResolveFrom<Settings>(Settings.SectionName);

        // TODO: remove this and replace it with utility from utilities library when https://jira.enigmatry.com/browse/ETL-349 is done
        private static T ResolveFrom<T>(this IConfiguration configuration, string sectionName)
        {
            var sectionSettings = configuration.GetSection(sectionName).Get<T>();
            return sectionSettings == null
                ? throw new InvalidOperationException($"Section is missing from configuration. Section Name: {sectionName}")
                : sectionSettings;
        }
    }
}
