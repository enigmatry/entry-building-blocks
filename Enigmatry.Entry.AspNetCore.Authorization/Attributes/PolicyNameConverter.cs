using System.ComponentModel;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;

internal static class PolicyNameConverter<T>
{
    private const char PermissionsDelimiter = ',';

    private static readonly TypeConverter Converter = TypeDescriptor.GetConverter(typeof(T));

    public static string ConvertToPolicyName(string policyPrefix, T[] permissions) =>
        $"{policyPrefix}{string.Join(PermissionsDelimiter, permissions.Select(ConvertToString))}";

    public static T[] ConvertFromPolicyName(string policyPrefix, string policyName) =>
        policyName[policyPrefix.Length..].Split(PermissionsDelimiter).Select(name => ConvertFromString(name)!).ToArray();

    public static bool CanConvertToPolicyName() => Converter.CanConvertTo(typeof(string));

    private static string? ConvertToString(T permission) => Converter.ConvertToString(permission);

    private static T? ConvertFromString(string permissionString) => (T?)Converter.ConvertFromString(permissionString);
}
