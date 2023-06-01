using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasRoleRequirement : IAuthorizationRequirement
{
    public const string PolicyPrefix = "UserHasRole";

    public UserHasRoleRequirement(string roles)
    {
        Roles = roles;
    }

    public string Roles { get; }
}
