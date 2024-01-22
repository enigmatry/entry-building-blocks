using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Quartz;

namespace Enigmatry.Entry.Scheduler;

internal sealed class ApplicationInsightsJobListener : IJobListener
{
    private readonly TelemetryClient _telemetryClient;

    public ApplicationInsightsJobListener(TelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        StartTelemetryOperation(context);
        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException,
        CancellationToken cancellationToken = default)
    {
        StopTelemetryOperation(context, jobException);
        return Task.CompletedTask;
    }

    private void StartTelemetryOperation(IJobExecutionContext context)
    {
        var telemetryOperation = _telemetryClient.StartOperation<RequestTelemetry>(context.JobDetail.Key.Name);
        context.Put(StorageKeyForTelemetry, telemetryOperation);
    }

    private void StopTelemetryOperation(IJobExecutionContext context, JobExecutionException? jobException = null)
    {
        using var telemetryOperation = context.Get(StorageKeyForTelemetry) as IOperationHolder<RequestTelemetry>;
        if (telemetryOperation == null)
        {
            return;
        }

        if (jobException != null)
        {
            // Both Success and Response code should be set in order for operation to show in the ApplicationInsights 'Failed' list
            telemetryOperation.Telemetry.Success = false;
            telemetryOperation.Telemetry.ResponseCode = "500";

            _telemetryClient.TrackException(jobException);
        }

        _telemetryClient.StopOperation(telemetryOperation);
    }

    private const string StorageKeyForTelemetry = "Telemetry_Operation_Holder";

    public string Name => nameof(ApplicationInsightsJobListener);
}
