using Enigmatry.Entry.Core.Helpers;
using MediatR;
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
            quartz.AddFeatures(configuration, assembly);
        });

        services.AddQuartzHostedService(quartz => quartz.WaitForJobsToComplete = true);
    }

    private static void AddFeatures(this IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration, Assembly assembly) =>
        assembly.GetTypes()
            .Where(type => !type.IsAbstract)
            .Where(type => type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>)))
            .Where(type => configuration.GetSchedulingFeatureSection(type).Exists())
            .Where(configuration.GetSchedulingFeatureRunValue)
            .ForEach(type => quartz.AddFeature(configuration, type));

    private static void AddFeature(this IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration, Type type)
    {
        var key = configuration.GetSchedulingFeatureSection(type).Key;

        quartz.AddJob(typeof(FeatureRunner<>).MakeGenericType(type), new JobKey(key));

        quartz.AddTrigger(trigger =>
        {
            var cronExpression = configuration.GetSchedulingFeatureCronExpressionValue(type)
                ?? throw new InvalidOperationException($"Cannot determine CRON expression for job: {key}");

            trigger.ForJob(key)
                .WithIdentity(key + "_Trigger")
                .WithCronSchedule(cronExpression);
        });

        if (configuration.GetSchedulingFeatureRunOnStartupValue(type))
        {
            quartz.AddTrigger(trigger =>
                trigger.ForJob(key)
                    .WithIdentity(key + "_RunOnStartup_Trigger")
                    .StartNow());
        }
    }
}
