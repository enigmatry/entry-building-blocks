using System.ComponentModel;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;

internal static class PolicyNameConverter<T>
{
    private const char PermissionsDelimiter = ',';

    private static readonly TypeConverter Converter = TypeDescriptor.GetConverter(typeof(T));

    public static bool CanConvert() => Converter.CanConvertTo(typeof(string));

    public static string ConvertToPolicyName(string policyPrefix, T[] permissions) =>
        $"{policyPrefix}{string.Join(PermissionsDelimiter, permissions.Select(ConvertToString))}";

    public static T[] ConvertFromPolicyName(string policyPrefix, string policyName) =>
        policyName[policyPrefix.Length..].Split(PermissionsDelimiter).Select(name => ConvertFromString(name)!).ToArray();

    private static string? ConvertToString(T permission) => Converter.ConvertToString(permission);

    private static T? ConvertFromString(string permissionString) => (T?)Converter.ConvertFromString(permissionString);
}
