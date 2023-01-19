using Enigmatry.Entry.Core.Helpers;
using Quartz;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.Scheduler;

public static class ServiceCollectionExtensions
{
    public static void AppAddQuartz(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        // Quartz configuration reference:
        // https://www.quartz-scheduler.net/documentation/quartz-3.x/configuration/reference.html#quartz-net-configuration-reference
        services.Configure<QuartzOptions>(configuration.GetSchedulingHostSection());

        services.AddQuartz(quartz =>
        {
            quartz.UseMicrosoftDependencyInjectionJobFactory();
            quartz.AddJobs(configuration, assembly);
        });

        services.AddQuartzHostedService(quartz => quartz.WaitForJobsToComplete = true);
    }

    private static void AddJobs(this IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration, Assembly assembly) =>
        assembly.GetTypes()
            .Where(type => !type.IsAbstract)
            .Where(type => type.GetInterface(nameof(IJob)) != null)
            .Where(type => configuration.GetSchedulingJobSection(type).Exists())
            .Where(configuration.GetSchedulingJobEnabledValue)
            .ForEach(type => quartz.AddJob(configuration, type));

    private static void AddJob(this IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration, Type type)
    {
        var key = configuration.GetSchedulingJobSection(type).Key;

        quartz.AddJob(type, new JobKey(key));

        quartz.AddTrigger(trigger =>
        {
            var cronExpression = configuration.GetSchedulingJobCronExpressionValue(type)
                ?? throw new InvalidOperationException($"Cannot determine CRON expression for job: {key}");

            trigger.ForJob(key)
                .WithIdentity(key + "_Trigger")
                .WithCronSchedule(cronExpression);
        });

        if (configuration.GetSchedulingJobRunOnStartupValue(type))
        {
            quartz.AddTrigger(trigger =>
                trigger.ForJob(key)
                    .WithIdentity(key + "_RunOnStartup_Trigger")
                    .StartNow());
        }
    }
}
