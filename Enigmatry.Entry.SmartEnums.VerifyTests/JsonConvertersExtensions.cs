using System.Reflection;
using Argon;
using JetBrains.Annotations;

namespace Enigmatry.Entry.SmartEnums.VerifyTests;

[PublicAPI]
public static class JsonConvertersExtensions
{
    /// <summary>
    /// Register SmartEnum converters for Argon (VerifyTests)
    /// </summary>
    /// <param name="converters">List of converters to add converters to</param>
    /// <param name="assembliesWithSmartEnums">Assemblies containing SmartEnums</param>
    public static void EntryAddSmartEnumJsonConverters(this IList<JsonConverter> converters, IEnumerable<Assembly> assembliesWithSmartEnums)
    {
        var smartEnums = assembliesWithSmartEnums.FindSmartEnums();

        foreach (var smartEnum in smartEnums)
        {
            var converterType =
                typeof(SmartEnumWriteOnlyJsonConverter<,>).MakeGenericType(smartEnum.EnumType, smartEnum.ValueType);
            var converter = (JsonConverter)Activator.CreateInstance(converterType)!;
            converters.Add(converter);
        }
    }
}
