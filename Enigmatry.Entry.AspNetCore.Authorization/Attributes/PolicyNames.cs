namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;

internal static class PolicyNames
{
    private const char PermissionsDelimiter = ',';

    public static string Format(string policyPrefix, IEnumerable<string> permissions) =>
        $"{policyPrefix}{string.Join(PermissionsDelimiter, permissions)}";

    public static IEnumerable<string> Parse(string policyPrefix, string policyName) =>
        policyName[policyPrefix.Length..].Split(PermissionsDelimiter);
}
