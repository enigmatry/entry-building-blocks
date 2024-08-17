using System.Reflection;
using Ardalis.SmartEnum;

namespace Enigmatry.Entry.SmartEnums;

// Taken from SmartEnum.EFCore
public static class SmartEnumTypeExtensions
{
    public static IEnumerable<(Type EnumType, Type ValueType)> FindSmartEnums(
        this IEnumerable<Assembly> assemblies) =>
        assemblies.SelectMany(a => a.GetTypes())
            .FilterSmartEnumTypes();

    private static IEnumerable<(Type EnumType, Type ValueType)> FilterSmartEnumTypes(this IEnumerable<Type> types)
    {
        var smartEnumsTypes = types.Where(t => t.IsDerivedFromSmartEnum());

        foreach (var smartEnumsType in smartEnumsTypes)
        {
            var (enumType, keyType) = GetEnumAndValueTypes(smartEnumsType, typeof(SmartEnum<,>));
            if (enumType != smartEnumsType)
            {
                // Only enum types 'TEnum' which extend SmartEnum<TEnum, TValue> are currently supported.
                continue;
            }

            if (keyType == null)
            {
                continue;
            }

            yield return (enumType, keyType);
        }
    }

    public static object[] GetSmartEnumValues(this Type type)
    {
        type.TryGetValues(out IEnumerable<object> enums);
        return enums.ToArray();
    }

    private static (Type? EnumType, Type? ValueType) GetEnumAndValueTypes(Type objectType, Type mainType)
    {
        var currentType = objectType.BaseType;

        if (currentType == null)
        {
            return (null, null);
        }

        while (currentType != null && currentType != typeof(object))
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == mainType)
            {
                return (currentType.GenericTypeArguments[0], currentType.GenericTypeArguments[1]);
            }

            currentType = currentType.BaseType;
        }

        return (null, null);
    }

    public static bool IsDerivedFromSmartEnum(this Type objectType) => IsDerived(objectType, typeof(SmartEnum<,>));

    private static bool IsDerived(Type objectType, Type mainType)
    {
        var currentType = objectType.BaseType;

        if (currentType == null)
        {
            return false;
        }

        while (currentType != null && currentType != typeof(object))
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == mainType)
            {
                return true;
            }

            currentType = currentType.BaseType;
        }

        return false;
    }
}
