﻿using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using JetBrains.Annotations;
using MemberTypes = System.Reflection.MemberTypes;

namespace Enigmatry.Entry.Csv;

[PublicAPI]
public class CsvHelper<T>
{
    private CsvHelperOptions _options;

    [Obsolete("Switch to creating CSVHelper using Options")]
    public CsvHelper() : this(_ => { })
    {
    }

    public CsvHelper(Action<CsvHelperOptions> optionsBuilder)
    {
        _options = new CsvHelperOptions();
        optionsBuilder(_options);
    }

    [Obsolete("Use same method without culture parameter. Switch to creating CSVHelper using Options")]
    public IEnumerable<T> GetRecords(Stream stream, CultureInfo culture)
    {
        _options = _options.WithCulture(culture);
        return GetRecords(stream);
    }

    public IEnumerable<T> GetRecords(Stream stream)
    {
        var classMap = CreateClassMap();
        using var textReader = new StreamReader(stream);
        using var reader = new CsvReader(textReader, _options.Culture);
        reader.Context.RegisterClassMap(classMap);
        return reader.GetRecords<T>().ToList();
    }

    [Obsolete("Use same method without culture parameter. Switch to creating CSVHelper using Options")]
    public byte[] WriteRecords(IEnumerable<T> records, CultureInfo culture)
    {
        _options = _options.WithCulture(culture);
        return WriteRecords(records);
    }

    public byte[] WriteRecords(IEnumerable<T> records)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var writer = new CsvWriter(streamWriter, _options.Culture);

        writer.Context.TypeConverterOptionsCache.AddOptions<DateTime>(
            new TypeConverterOptions { Formats = ["yyyy-MM-dd HH:mm:ss"] });
        writer.Context.TypeConverterCache.AddConverter<DateTimeOffset>(
            new DateTimeOffsetToLocalDateTimeConverter());

        var classMap = CreateClassMap();
        writer.Context.RegisterClassMap(classMap);

        writer.WriteRecords(records);
        streamWriter.Flush();
        return memoryStream.ToArray();
    }

    private DefaultClassMap<T> CreateClassMap()
    {
        var classMap = new DefaultClassMap<T>();
        classMap.AutoMap(_options.Culture);

        foreach (var member in typeof(T)
                     .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                     .Where(m => m.MemberType is MemberTypes.Property or MemberTypes.Field))
        {
            if (member.GetCustomAttribute<IgnoreAttribute>() != null)
            {
                classMap.Map(typeof(T), member).Ignore();
            }

            var name = _options.HeaderNameReplacer(member.Name);
            var nameAttribute = member.GetCustomAttribute<NameAttribute>();
            if (nameAttribute != null)
            {
                name = _options.HeaderNameReplacer(nameAttribute.Name);
            }

            classMap.Map(typeof(T), member).Name(name);
        }

        return classMap;
    }
}
