using Microsoft.Extensions.Configuration;

namespace Enigmatry.Entry.Scheduler;

internal static class ConfigurationExtensions
{
    internal static IConfigurationSection GetSchedulingSection(this IConfiguration configuration) =>
        configuration.GetSection("Scheduling");

    internal static IConfigurationSection GetSchedulingHostSection(this IConfiguration configuration) =>
        configuration.GetSchedulingSection().GetSection("Host");

    internal static IConfigurationSection GetSchedulingFeatureSection(this IConfiguration configuration, Type feature) =>
        configuration.GetSchedulingFeatureSection(GetFeatureKey(feature));

    internal static string? GetSchedulingFeatureCronExpressionValue(this IConfiguration configuration, Type feature) =>
        configuration.GetSchedulingFeatureSection(feature)["Cronex"];

    internal static T GetSchedulingFeatureArgumentsValue<T>(this IConfiguration configuration) where T : new()
    {
        var section = configuration.GetSchedulingFeatureSection(typeof(T)).GetSection("Args");
        return section.Value == null ? new T() : section.Get<T>();
    }

    internal static bool GetSchedulingFeatureRunValue(this IConfiguration configuration, Type feature) =>
        configuration.GetSchedulingFeatureSection(feature).GetValue<bool?>("Run") ?? true;

    internal static bool GetSchedulingFeatureRunOnStartupValue(this IConfiguration configuration, Type feature) =>
        configuration.GetSchedulingFeatureSection(feature).GetValue<bool?>("RunOnStartup") ?? false;

    private static IConfigurationSection GetSchedulingFeatureSection(this IConfiguration configuration, string key) =>
        configuration.GetSchedulingSection().GetSection($"Features:{key}");

    private static string GetFeatureKey(Type type) => type.ReflectedType!.Name;
}
