using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Enigmatry.Entry.Scheduler;

public abstract class EntryJob<T> : IJob where T : class, new()
{
    private readonly ILogger<EntryJob<T>> _logger;
    private readonly IConfiguration _configuration;

    protected EntryJob(ILogger<EntryJob<T>> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public virtual async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation($"Processing {jobName} job...");

        try
        {
            await Execute(_configuration.GetSchedulingJobArgumentsValue<T>());
            _logger.LogInformation($"{jobName} job completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred during {jobName} job execution.");
        }
    }

    public abstract Task Execute(T request);
}
