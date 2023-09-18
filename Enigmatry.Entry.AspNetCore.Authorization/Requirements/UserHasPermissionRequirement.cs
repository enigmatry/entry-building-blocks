using Enigmatry.Entry.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasPermissionRequirement<TPermission> : IAuthorizationRequirement where TPermission : notnull
{
    public UserHasPermissionRequirement(IEnumerable<TPermission> permissions)
    {
        Permissions = permissions;
    }

    public IEnumerable<TPermission> Permissions { get; }

    public override string ToString() =>
        $"{nameof(UserHasPermissionRequirement<TPermission>)}: " +
        $"{string.Join(",", Permissions.Select(PermissionTypeConverter<TPermission>.ConvertToString))}";
}
