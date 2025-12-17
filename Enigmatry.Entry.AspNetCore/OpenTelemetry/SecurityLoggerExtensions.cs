using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.OpenTelemetry;
public static class SecurityLoggerExtensions
{
    // When changing name parameter don't forget to change the log filter expression in the appsettings.json
    private static readonly EventId SecurityEventId = new(999, "SecurityLog");

    public static void LogSecurityTrace(this ILogger logger, string message, params object?[] args) =>
        logger.LogTrace(SecurityEventId, message, args);

    public static void LogSecurityDebug(this ILogger logger, string message, params object?[] args) =>
        logger.LogDebug(SecurityEventId, message, args);

    public static void LogSecurityInformation(this ILogger logger, string message, params object?[] args) =>
        logger.LogInformation(SecurityEventId, message, args);

    public static void LogSecurityWarning(this ILogger logger, string message, params object?[] args) =>
        logger.LogWarning(SecurityEventId, message, args);

    public static void LogSecurityError(this ILogger logger, Exception exception, string message, params object?[] args) =>
        logger.LogError(SecurityEventId, exception, message, args);

    public static void LogSecurityCritical(this ILogger logger, string message, params object?[] args) =>
        logger.LogCritical(SecurityEventId, message, args);
}
