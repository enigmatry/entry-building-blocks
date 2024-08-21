using Ardalis.SmartEnum;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enigmatry.Entry.SmartEnums.EntityFramework;

[PublicAPI]
public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Adds a conversion for a SmartEnum to an integer
    /// </summary>
    /// <param name="propertyBuilder">Property builder</param>
    /// <typeparam name="TProperty">Type of property</typeparam>
    /// <returns></returns>
    public static PropertyBuilder<TProperty> HasSmartEnumConversion<TProperty>(
        this PropertyBuilder<TProperty> propertyBuilder)
        where TProperty : SmartEnum<TProperty, int> =>
        propertyBuilder.HasSmartEnumConversion<TProperty, int>();

    /// <summary>
    /// Adds a conversion for a nullable SmartEnum to an integer
    /// </summary>
    /// <param name="propertyBuilder">Property builder</param>
    /// <typeparam name="TProperty">Type of property</typeparam>
    public static void HasNullableSmartEnumConversion<TProperty>(this PropertyBuilder<TProperty?> propertyBuilder)
        where TProperty : SmartEnum<TProperty, int> =>
        propertyBuilder.HasConversion(
            p => p == null ? (int?)null : p.Value,
            p => p.HasValue ? SmartEnum<TProperty>.FromValue(p.Value) : null);

    /// <summary>
    /// Adds a conversion for a SmartEnum to a value
    /// </summary>
    /// <param name="propertyBuilder">Property builder</param>
    /// <typeparam name="TProperty">Type of property</typeparam>
    /// <typeparam name="TValue">Type of value</typeparam>
    /// <returns></returns>
    public static PropertyBuilder<TProperty> HasSmartEnumConversion<TProperty, TValue>(
        this PropertyBuilder<TProperty> propertyBuilder)
        where TProperty : SmartEnum<TProperty, TValue> where TValue : IEquatable<TValue>, IComparable<TValue> =>
        propertyBuilder.HasConversion(
            p => p.Value,
            p => SmartEnum<TProperty, TValue>.FromValue(p));
}
