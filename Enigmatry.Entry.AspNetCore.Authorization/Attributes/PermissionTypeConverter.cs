using System.ComponentModel;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;

/// <summary>
/// Helper class for converting permissions to and from string (policy name).
/// </summary>
/// <typeparam name="TPermission"></typeparam>
internal static class PermissionTypeConverter<TPermission> where TPermission : notnull
{
    private const char PermissionsDelimiter = '|';
    private static readonly TypeConverter TypeConverter = TypeDescriptor.GetConverter(typeof(TPermission));

    public static void EnsureConversionToPolicyNameIsPossible()
    {
        if (!TypeConverter.CanConvertTo(typeof(string)) || !TypeConverter.CanConvertFrom(typeof(string)))
        {
            throw new ArgumentException(
                $"Permission type {typeof(TPermission)} can not be converted to or from string. You need to implement {typeof(TypeConverter)} when using custom permission type");
        }
    }

    public static string ConvertToPolicyName(string policyPrefix, TPermission[] permissions) =>
        $"{policyPrefix}{string.Join(PermissionsDelimiter, permissions.Select(ConvertToString))}";

    public static TPermission[] ConvertFromPolicyName(string policyPrefix, string policyName) =>
        policyName[policyPrefix.Length..].Split(PermissionsDelimiter)
            .Select(ConvertFromString)
            .ToArray();

    public static string ConvertToString(TPermission permission)
    {
        if (permission == null)
        {
            throw new ArgumentNullException(nameof(permission));
        }

        return TypeConverter.ConvertToString(permission)!;
    }

    public static TPermission ConvertFromString(string permissionString)
    {
        if (string.IsNullOrEmpty(permissionString))
        {
            throw new ArgumentNullException(nameof(permissionString));
        }

        return (TPermission)TypeConverter.ConvertFromString(permissionString)!;
    }
}
