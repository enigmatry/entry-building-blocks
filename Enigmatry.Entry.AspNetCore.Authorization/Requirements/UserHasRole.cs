using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasRoleRequirement : IAuthorizationRequirement
{
    public UserHasRoleRequirement(string roles)
    {
        Roles = roles;
    }

    public string Roles { get; }
}

internal class UserHasRoleRequirementHandler : AuthenticatedUserRequirementHandler<UserHasRoleRequirement>
{
    private readonly IAuthorizationProvider _authorizationProvider;

    public UserHasRoleRequirementHandler(
        IAuthorizationProvider authorizationProvider,
        ILogger<AuthenticatedUserRequirementHandler<UserHasRoleRequirement>> logger)
        : base(logger)
    {
        _authorizationProvider = authorizationProvider;
    }

    protected override bool FulfillsRequirement(AuthorizationHandlerContext context, UserHasRoleRequirement requirement) =>
        _authorizationProvider.HasAnyRole(requirement.Roles.Split(','));
}
