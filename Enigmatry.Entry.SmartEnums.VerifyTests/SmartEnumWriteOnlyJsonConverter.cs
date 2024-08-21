using Ardalis.SmartEnum;

namespace Enigmatry.Entry.SmartEnums.VerifyTests;

/// <summary>
/// Json Converter that will write SmartEnum as a string (uses Name property) 
/// </summary>
/// <typeparam name="TEnum">Type of enum</typeparam>
/// <typeparam name="TValue">Type of value</typeparam>
internal class SmartEnumWriteOnlyJsonConverter<TEnum, TValue> : WriteOnlyJsonConverter<TEnum>
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public override void Write(VerifyJsonWriter writer, TEnum? value)
    {
        if (value == null)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteValue(value.Name);
        }
    }
}
