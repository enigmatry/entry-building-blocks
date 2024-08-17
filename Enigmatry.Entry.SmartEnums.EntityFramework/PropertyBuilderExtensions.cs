using Ardalis.SmartEnum;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enigmatry.Entry.SmartEnums.EntityFramework;

[PublicAPI]
public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<TProperty> HasSmartEnumConversion<TProperty>(
        this PropertyBuilder<TProperty> propertyBuilder)
        where TProperty : SmartEnum<TProperty, int> =>
        propertyBuilder.HasSmartEnumConversion<TProperty, int>();

    public static void HasNullableSmartEnumConversion<TProperty>(this PropertyBuilder<TProperty?> propertyBuilder)
        where TProperty : SmartEnum<TProperty, int> =>
        propertyBuilder.HasConversion(
            p => p == null ? (int?)null : p.Value,
            p => p.HasValue ? SmartEnum<TProperty>.FromValue(p.Value) : null);
    public static PropertyBuilder<TProperty> HasSmartEnumConversion<TProperty, TValue>(
        this PropertyBuilder<TProperty> propertyBuilder)
        where TProperty : SmartEnum<TProperty, TValue> where TValue : IEquatable<TValue>, IComparable<TValue> =>
        propertyBuilder.HasConversion(
            p => p.Value,
            p => SmartEnum<TProperty, TValue>.FromValue(p));
}
