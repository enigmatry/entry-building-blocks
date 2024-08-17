using Ardalis.SmartEnum;

namespace Enigmatry.Entry.SmartEnums.VerifyTests;

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
