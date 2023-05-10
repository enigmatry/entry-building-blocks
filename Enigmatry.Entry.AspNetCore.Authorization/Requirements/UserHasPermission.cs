using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasPermissionRequirement : IAuthorizationRequirement
{
    public UserHasPermissionRequirement(string permissions)
    {
        Permissions = permissions;
    }

    public string Permissions { get; }
}

internal class UserHasPermissionRequirementHandler : AuthenticatedUserRequirementHandler<UserHasPermissionRequirement>
{
    private readonly IAuthorizationProvider _authorizationProvider;

    public UserHasPermissionRequirementHandler(
        IAuthorizationProvider authorizationProvider,
        ILogger<AuthenticatedUserRequirementHandler<UserHasPermissionRequirement>> logger)
        : base(logger)
    {
        _authorizationProvider = authorizationProvider;
    }

    protected override bool FulfillsRequirement(AuthorizationHandlerContext context, UserHasPermissionRequirement requirement) =>
        _authorizationProvider.HasAnyPermission(requirement.Permissions.Split(','));
}
