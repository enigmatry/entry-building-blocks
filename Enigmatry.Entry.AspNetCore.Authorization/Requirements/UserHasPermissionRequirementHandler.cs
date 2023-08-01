using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasPermissionRequirementHandler<TPermission> : AuthenticatedUserRequirementHandler<UserHasPermissionRequirement<TPermission>> where TPermission : notnull
{
    private readonly IAuthorizationProvider<TPermission> _authorizationProvider;

    public UserHasPermissionRequirementHandler(
        IAuthorizationProvider<TPermission> authorizationProvider,
        ILogger<AuthenticatedUserRequirementHandler<UserHasPermissionRequirement<TPermission>>> logger)
        : base(logger)
    {
        _authorizationProvider = authorizationProvider;
    }

    protected override bool FulfillsRequirement(AuthorizationHandlerContext context, UserHasPermissionRequirement<TPermission> requirement) =>
        _authorizationProvider.AuthorizePermissions(requirement.Permissions);
}
