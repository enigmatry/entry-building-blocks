namespace Enigmatry.Entry.EntityFramework.MediatR;

public static class GenericTypeExtensions
{
    public static string GetGenericTypeName(this object obj) => obj.GetType().GetGenericTypeName();

    private static string GetGenericTypeName(this Type type)
    {
        string typeName;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name));
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`', StringComparison.Ordinal))}<{genericTypes}>";
        }
        else
        {
            typeName = type.DeclaringType != null ? $"{type.DeclaringType.Name}.{type.Name}" : type.Name;
        }

        return typeName;
    }
}
