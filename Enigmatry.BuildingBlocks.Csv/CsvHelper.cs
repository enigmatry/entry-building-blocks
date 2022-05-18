using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MemberTypes = System.Reflection.MemberTypes;

namespace Enigmatry.BuildingBlocks.Csv
{
    public class CsvHelper<T>
    {
        public IEnumerable<T> GetRecords(Stream stream, CultureInfo culture)
        {
            using var textReader = new StreamReader(stream);
            using var reader = new CsvReader(textReader, culture);
            return reader.GetRecords<T>().ToList();
        }

        public byte[] WriteRecords(IEnumerable<T> records, CultureInfo culture)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);
            using var writer = new CsvWriter(streamWriter, culture);

            writer.Context.TypeConverterOptionsCache.AddOptions<DateTime>(
                new TypeConverterOptions { Formats = new[] { "yyyy-MM-dd HH:mm:ss" } });
            writer.Context.TypeConverterCache.AddConverter<DateTimeOffset>(
                new DateTimeOffsetToLocalDateTimeConverter());

            var classMap = CreateClassMap(culture);
            writer.Context.RegisterClassMap(classMap);

            writer.WriteRecords(records);
            streamWriter.Flush();
            return memoryStream.ToArray();
        }

        private static ClassMap CreateClassMap(CultureInfo culture)
        {
            var classMap = new DefaultClassMap<T>();
            classMap.AutoMap(culture);

            foreach (var member in typeof(T)
                         .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                         .Where(m => m.MemberType is MemberTypes.Property or MemberTypes.Field))
            {
                if (member.GetCustomAttribute<IgnoreAttribute>() != null)
                {
                    classMap.Map(typeof(T), member).Ignore();
                }

                var nameAttribute = member.GetCustomAttribute<NameAttribute>();
                if (nameAttribute != null)
                {
                    classMap.Map(typeof(T), member).Name(nameAttribute.Name);
                }
            }

            return classMap;
        }
    }
}
