using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasRoleRequirement : IAuthorizationRequirement
{
    public UserHasRoleRequirement(string roles)
    {
        Roles = roles;
    }

    // Array!
    public string Roles { get; }
}
