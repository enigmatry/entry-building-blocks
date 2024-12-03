using System.Reflection;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using JetBrains.Annotations;

namespace Enigmatry.Entry.SmartEnums.SystemTextJson;

[PublicAPI]
public static class JsonConverterExtensions
{
    /// <summary>
    /// Register SmartEnum json converters for System.Text.Json
    /// </summary>
    /// <param name="converters">List of converters to add converters to</param>
    /// <param name="smartEnumConverterType">The type of converter to use</param>
    /// <param name="assembliesWithSmartEnums">Assemblies containing SmartEnums</param>
    public static void EntryAddSmartEnumJsonConverters(this IList<JsonConverter> converters,
        SmartEnumConverterType smartEnumConverterType, IEnumerable<Assembly> assembliesWithSmartEnums)
    {
        var smartEnums = assembliesWithSmartEnums.FindSmartEnums();

        foreach (var smartEnumsType in smartEnums)
        {
            var typeOfConverter = smartEnumConverterType == SmartEnumConverterType.NameConverter
                ? typeof(SmartEnumNameConverter<,>)
                : typeof(SmartEnumValueConverter<,>);

            var converterType = typeOfConverter.MakeGenericType(smartEnumsType.EnumType, smartEnumsType.ValueType);
            var converter = (JsonConverter)Activator.CreateInstance(converterType)!;
            converters.Add(converter);
        }
    }
}
