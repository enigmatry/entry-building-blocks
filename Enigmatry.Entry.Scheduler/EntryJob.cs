﻿using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Enigmatry.Entry.Scheduler;

[PublicAPI]
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
            var jobType = GetType();
            await Execute(_configuration.GetJobConfiguration(jobType).GetSchedulingJobArgumentsValue<T>());
            _logger.LogInformation($"{jobName} job completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred during {jobName} job execution.");
        }
    }

    public abstract Task Execute(T request);
}
