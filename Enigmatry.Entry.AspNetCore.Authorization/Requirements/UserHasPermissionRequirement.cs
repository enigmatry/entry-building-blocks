using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasPermissionRequirement<T> : IAuthorizationRequirement
{
    public UserHasPermissionRequirement(IEnumerable<T> permissions)
    {
        Permissions = permissions;
    }

    public IEnumerable<T> Permissions { get; }
}
