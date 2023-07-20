using System.ComponentModel;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;

internal static class PermissionsConverter<T>
{
    private const char PermissionsDelimiter = ',';

    private static readonly TypeConverter Converter = TypeDescriptor.GetConverter(typeof(T));

    public static string FormatToPolicyName(string policyPrefix, T[] permissions) =>
        $"{policyPrefix}{string.Join(PermissionsDelimiter, permissions.Select(ConvertToString))}";

    public static T[] ParseFromPolicyName(string policyPrefix, string policyName) =>
        policyName[policyPrefix.Length..].Split(PermissionsDelimiter).Select(name => ConvertFromString(name)!).ToArray();

    public static void EnsurePermissionTypeCanBeConverted()
    {
        if (!Converter.CanConvertTo(typeof(string)))
        {
            throw new ArgumentException(
                $"Permission type {typeof(T)} can not be converted to string. You need to implement {typeof(TypeConverter)} when using custom permission type");
        }
    }

    private static string? ConvertToString(T permission) => Converter.ConvertToString(permission);

    private static T? ConvertFromString(string permissionString) => (T?)Converter.ConvertFromString(permissionString);
}
