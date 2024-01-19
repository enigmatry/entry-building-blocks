using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Quartz;

namespace Enigmatry.Entry.Scheduler;

internal sealed class ApplicationInsightsJobListener : IJobListener, IDisposable
{
    private readonly TelemetryClient _telemetryClient;
    private readonly Dictionary<JobKey, IOperationHolder<RequestTelemetry>> _telemetryOperations = [];

    public ApplicationInsightsJobListener(TelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        StartTelemetryOperation(context);
        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        StopTelemetryOperation(context);
        return Task.CompletedTask;
    }

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException,
        CancellationToken cancellationToken = default)
    {
        StopTelemetryOperation(context, jobException);
        return Task.CompletedTask;
    }

    private void StartTelemetryOperation(IJobExecutionContext context)
    {
        var operation = _telemetryClient.StartOperation<RequestTelemetry>(context.JobDetail.Key.Name);
        _telemetryOperations.TryAdd(context.JobDetail.Key, operation);
    }

    private void StopTelemetryOperation(IJobExecutionContext context, JobExecutionException? jobException = null)
    {
        if (!_telemetryOperations.TryGetValue(context.JobDetail.Key, out var operation))
        {
            return;
        }

        if (jobException is not null)
        {
            // Response code needs to be set in order for operation to show in ApplicationInsights 'Failed' list
            operation.Telemetry.Success = false;
            operation.Telemetry.ResponseCode = jobException.GetType().Name;
        }

        _telemetryClient.StopOperation(operation);
        _telemetryOperations.Remove(context.JobDetail.Key);
    }

    public void Dispose() => _telemetryClient.Flush();

    public string Name => nameof(ApplicationInsightsJobListener);
}
