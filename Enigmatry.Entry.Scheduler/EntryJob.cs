using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Enigmatry.Entry.Scheduler;

[PublicAPI]
public abstract class EntryJob<T>(ILogger<EntryJob<T>> logger, IConfiguration configuration) : IJob
    where T : class, new()
{
    public virtual async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        logger.LogInformation("Processing {JobName} job...", jobName);

        try
        {
            var jobType = GetType();
            await Execute(configuration.GetJobConfiguration(jobType).GetSchedulingJobArgumentsValue<T>());
            logger.LogInformation("{JobName} job completed.", jobName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during {JobName} job execution.", jobName);
        }
    }

    public abstract Task Execute(T request);
}
