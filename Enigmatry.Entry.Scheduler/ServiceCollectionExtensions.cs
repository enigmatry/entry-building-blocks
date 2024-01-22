using Enigmatry.Entry.Core.Helpers;
using Quartz;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.Scheduler;

[PublicAPI]
public static class ServiceCollectionExtensions
{
    [Obsolete("Use AddEntryQuartz instead")]
    public static void AppAddQuartz(this IServiceCollection services, IConfiguration configuration, Assembly assembly,
        ILogger logger) =>
        services.AddEntryQuartz(configuration, assembly, logger);

    public static void AddEntryQuartz(this IServiceCollection services, IConfiguration configuration, Assembly assembly,
        ILogger logger, Action<IServiceCollectionQuartzConfigurator>? configure = null)
    {
        // Quartz configuration reference:
        // https://www.quartz-scheduler.net/documentation/quartz-3.x/configuration/reference.html#quartz-net-configuration-reference
        services.Configure<QuartzOptions>(configuration.GetSchedulingHostSection());

        services.AddQuartz(quartz =>
        {
            quartz.AddJobs(configuration, assembly, logger);
            configure?.Invoke(quartz);
        });

        services.AddQuartzHostedService(quartz => quartz.WaitForJobsToComplete = true);
    }

    public static void AddEntryApplicationInsights(this IServiceCollectionQuartzConfigurator configurator) => configurator.AddJobListener<ApplicationInsightsJobListener>();

    private static void AddJobs(this IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration,
        Assembly assembly, ILogger logger)
    {
        var jobTypes = assembly.FindAllJobTypes();
        var configurations = configuration.FindAllJobConfigurations(jobTypes);
        configurations.ForEach(section => quartz.AddJob(section, logger));
    }

    private static void AddJob(this IServiceCollectionQuartzConfigurator quartz, JobConfiguration config,
        ILogger logger)
    {
        var settings = config.Settings;
        if (!settings.Enabled)
        {
            logger.LogWarning("Job: {JobName} is disabled. Skipping registration",
                config.JobName);
            return;
        }

        var key = config.JobName;
        quartz.AddJob(config.JobType, new JobKey(key));

        quartz.AddTrigger(trigger =>
        {
            trigger.ForJob(key)
                .WithIdentity(key + "_Trigger")
                .WithCronSchedule(settings.Cronex);
        });

        if (settings.RunOnStartup)
        {
            quartz.AddTrigger(trigger =>
                trigger.ForJob(key)
                    .WithIdentity(key + "_RunOnStartup_Trigger")
                    .StartNow());
        }
    }
}
