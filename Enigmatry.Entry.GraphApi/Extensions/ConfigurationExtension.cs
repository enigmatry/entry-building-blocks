using Enigmatry.Entry.Core.Settings;
using Microsoft.Extensions.Configuration;

namespace Enigmatry.Entry.GraphApi.Extensions;
internal static class ConfigurationExtension
{
    internal static GraphApiSettings ResolveGraphApiSettings(this IConfiguration configuration)
        => configuration.ResolveFrom<GraphApiSettings>(GraphApiSettings.GraphApiSectionName);

    // TODO: remove this and replace it with utility from utilities library when https://jira.enigmatry.com/browse/ETL-349 is done
    private static T ResolveFrom<T>(this IConfiguration configuration, string sectionName)
    {
        var sectionSettings = configuration.GetSection(sectionName).Get<T>();
        return sectionSettings == null
            ? throw new InvalidOperationException($"Section is missing from configuration. Section Name: {sectionName}")
            : sectionSettings;
    }
}
