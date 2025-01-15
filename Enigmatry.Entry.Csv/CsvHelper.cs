using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
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
        using var streamWriter = new StreamWriter(memoryStream, _options.Encoding);
        using var writer = new CsvWriter(streamWriter, _options.Culture);

        WriteRecordsToStream(records, memoryStream, streamWriter, writer);

        return memoryStream.ToArray();
    }

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
    public MemoryStream WriteRecordsToStream(IEnumerable<T> records)
    {
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
        var writer = new CsvWriter(streamWriter, _options.Culture);

        WriteRecordsToStream(records, memoryStream, streamWriter, writer);

        return memoryStream;
    }

    private MemoryStream WriteRecordsToStream(IEnumerable<T> records, MemoryStream memoryStream, StreamWriter streamWriter,
        CsvWriter csvWriter)
    {
        csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime>(
            new TypeConverterOptions { Formats = ["yyyy-MM-dd HH:mm:ss"] });
        csvWriter.Context.TypeConverterCache.AddConverter<DateTimeOffset>(
            new DateTimeOffsetToLocalDateTimeConverter());

        var classMap = CreateClassMap();
        csvWriter.Context.RegisterClassMap(classMap);

        csvWriter.WriteRecords(records);
        streamWriter.Flush();
        memoryStream.Position = 0;

        return memoryStream;
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
