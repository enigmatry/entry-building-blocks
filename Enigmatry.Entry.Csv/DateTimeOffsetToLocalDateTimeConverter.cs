using System;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Enigmatry.Entry.Csv
{
    public class DateTimeOffsetToLocalDateTimeConverter : DefaultTypeConverter
    {
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData) =>
            value is DateTimeOffset dateTimeOffset
                ? dateTimeOffset.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                : base.ConvertToString(value, row, memberMapData);
    }
}
