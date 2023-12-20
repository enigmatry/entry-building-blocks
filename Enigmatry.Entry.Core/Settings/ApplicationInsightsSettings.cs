using System;
using JetBrains.Annotations;
using Serilog.Events;

namespace Enigmatry.Entry.Core.Settings
{
    [UsedImplicitly]
    public class ApplicationInsightsSettings
    {
        public const string ApplicationInsightsSectionName = "ApplicationInsights";

        public string InstrumentationKey { get; set; } = string.Empty;
        public LogEventLevel SerilogLogsRestrictedToMinimumLevel { get; set; }
    }
}
