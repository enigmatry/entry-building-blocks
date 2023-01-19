using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Enigmatry.Entry.Scheduler;

internal static class ConfigurationExtensions
{
    internal static IConfigurationSection GetSchedulingSection(this IConfiguration configuration) =>
        configuration.GetSection("Scheduling");

    internal static IConfigurationSection GetSchedulingHostSection(this IConfiguration configuration) =>
        configuration.GetSchedulingSection().GetSection("Host");

    internal static IConfigurationSection GetSchedulingJobSection(this IConfiguration configuration, Type job) =>
        configuration.GetSchedulingJobSection(GetJobKey(job));

    internal static string? GetSchedulingJobCronExpressionValue(this IConfiguration configuration, Type job) =>
        configuration.GetSchedulingJobSection(job)["Cronex"];

    internal static T GetSchedulingJobArgumentsValue<T>(this IConfiguration configuration) where T : new()
    {
        var section = configuration.GetSchedulingJobSection(typeof(T).ReflectedType).GetSection("Args");
        return section == null ? new T() : section.Get<T>();
    }

    internal static bool GetSchedulingJobEnabledValue(this IConfiguration configuration, Type job) =>
        configuration.GetSchedulingJobSection(job).GetValue<bool?>("Enabled") ?? true;

    internal static bool GetSchedulingJobRunOnStartupValue(this IConfiguration configuration, Type job) =>
        configuration.GetSchedulingJobSection(job).GetValue<bool?>("RunOnStartup") ?? false;

    private static IConfigurationSection GetSchedulingJobSection(this IConfiguration configuration, string key)
    {
        var jobSection = configuration.GetSchedulingSection().GetSection($"Jobs:{key}");
        if (jobSection.Exists())
        {
            return configuration.GetSchedulingSection().GetSection($"Jobs:{key}");
        }

        throw new ConfigurationErrorsException($"Job {key} is not configured correctly.");
    }

    private static string GetJobKey(Type type) => type.Name;
}
