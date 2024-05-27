using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Quartz;

namespace Enigmatry.Entry.Scheduler;

internal sealed class ApplicationInsightsJobListener : IJobListener
{
    private readonly TelemetryClient _telemetryClient;

    public ApplicationInsightsJobListener(TelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        TrackJobExecution(context, jobException);
        return Task.CompletedTask;
    }

    private void TrackJobExecution(IJobExecutionContext context, JobExecutionException? jobException = null)
    {
        var jobName = context.JobDetail.Key.Name;

        var requestTelemetry = new RequestTelemetry
        {
            Name = jobName,
            Timestamp = context.FireTimeUtc,
            Duration = context.JobRunTime
        };

        requestTelemetry.GenerateOperationId();
        requestTelemetry.Context.Operation.Name = jobName;

        if (jobException != null)
        {
            requestTelemetry.ResponseCode = "500";
            requestTelemetry.Success = false;

            _telemetryClient.TrackException(jobException);
        }

        _telemetryClient.TrackRequest(requestTelemetry);
    }

    public string Name => nameof(ApplicationInsightsJobListener);
}
