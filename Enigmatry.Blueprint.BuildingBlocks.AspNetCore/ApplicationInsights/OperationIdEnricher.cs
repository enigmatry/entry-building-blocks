using Serilog.Core;
using Serilog.Events;

namespace Enigmatry.Blueprint.BuildingBlocks.AspNetCore.ApplicationInsights
{
    // added because of AppInsights sampling of custom events -
    // ensures that a set of events is retained or discarded together
    // https://github.com/serilog/serilog-sinks-applicationinsights#including-operation-id
    // and 
    // https://docs.microsoft.com/en-us/azure/azure-monitor/app/sampling#in-brief
    public class OperationIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Properties.TryGetValue("RequestId", out var requestId))
            {
                logEvent.AddPropertyIfAbsent(new LogEventProperty("operationId", requestId));
            }
        }
    }
}
