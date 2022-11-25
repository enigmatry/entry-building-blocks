using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Enigmatry.Entry.Scheduler;

public class FeatureRunner<T> : IJob where T : IBaseRequest, new()
{
    private readonly ILogger<FeatureRunner<T>> _logger;
    private readonly ISender _sender;
    private readonly IConfiguration _configuration;

    public FeatureRunner(
        ILogger<FeatureRunner<T>> logger,
        ISender sender,
        IConfiguration configuration)
    {
        _logger = logger;
        _sender = sender;
        _configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var featureName = context.JobDetail.Key.Name;
        _logger.LogInformation("Processing {Feature} feature...", featureName);

        try
        {
            await _sender.Send(_configuration.GetSchedulingFeatureArgumentsValue<T>(), context.CancellationToken);
            _logger.LogInformation("{Feature} feature completed.", featureName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during {Feature} feature execution.", featureName);
        }
    }
}

