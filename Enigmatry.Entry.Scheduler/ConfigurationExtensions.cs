using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Enigmatry.Entry.Scheduler;

internal static class ConfigurationExtensions
{
    internal static IEnumerable<JobConfiguration> FindAllJobConfigurations(this IConfiguration configuration,
        IEnumerable<Type> types) =>
        types.Select(configuration.GetJobConfiguration);

    public static JobConfiguration GetJobConfiguration(this IConfiguration configuration, Type jobType)
    {
        var jobName = JobConfiguration.GetJobName(jobType);
        var schedulingSection = configuration.GetSchedulingSection();
        var jobSection = schedulingSection.GetSection($"Jobs:{jobName}");
        var settings = jobSection.Get<JobSettings>();
        return new JobConfiguration(jobName, jobType, settings, jobSection);
    }

    private static IConfigurationSection GetSchedulingSection(this IConfiguration configuration)
    {
        const string sectionName = "Scheduling";
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            throw new ConfigurationErrorsException($"Section '{sectionName}' is missing from the configuration");
        }

        return section;
    }

    internal static IConfigurationSection GetSchedulingHostSection(this IConfiguration configuration) =>
        configuration.GetSchedulingSection().GetSection("Host");
}
