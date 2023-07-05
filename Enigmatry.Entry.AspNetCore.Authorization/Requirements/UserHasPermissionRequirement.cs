using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasPermissionRequirement : IAuthorizationRequirement
{
    public UserHasPermissionRequirement(IEnumerable<string> permissions)
    {
        Permissions = permissions;
    }

    public IEnumerable<string> Permissions { get; }
}
