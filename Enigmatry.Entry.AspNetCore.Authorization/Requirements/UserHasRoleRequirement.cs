using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasRoleRequirement : IAuthorizationRequirement
{
    public UserHasRoleRequirement(IEnumerable<string> roles)
    {
        Roles = roles;
    }

    public IEnumerable<string> Roles { get; }
}
