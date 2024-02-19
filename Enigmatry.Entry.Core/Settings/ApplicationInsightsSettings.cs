using System;
using JetBrains.Annotations;
using Serilog.Events;

namespace Enigmatry.Entry.Core.Settings;

[PublicAPI]
public class ApplicationInsightsSettings
{
    public const string ApplicationInsightsSectionName = "ApplicationInsights";

    [Obsolete("Migrate to use ConnectionString instead.")]
    public string InstrumentationKey { get; set; } = string.Empty;

    public string ConnectionString { get; set; } = string.Empty;

    [Obsolete("Switch to new way of integrating AppInsights with Serilog - https://github.com/serilog-contrib/serilog-sinks-applicationinsights.")]
    public LogEventLevel SerilogLogsRestrictedToMinimumLevel { get; set; }
}
