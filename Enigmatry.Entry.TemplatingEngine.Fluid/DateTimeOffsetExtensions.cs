using TimeZoneConverter;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public static class DateTimeOffsetExtensions
{
    public static readonly TimeZoneInfo WestEuropeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard time");

    public static DateTime ToDutchDateTime(this DateTimeOffset dateTimeOffset) =>
        TimeZoneInfo.ConvertTimeFromUtc(dateTimeOffset.UtcDateTime.ToUniversalTime(), WestEuropeZone);
}
