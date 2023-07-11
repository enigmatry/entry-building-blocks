using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasPermissionRequirementHandler<T> : AuthenticatedUserRequirementHandler<UserHasPermissionRequirement<T>>
{
    private readonly IAuthorizationProvider<T> _authorizationProvider;

    public UserHasPermissionRequirementHandler(
        IAuthorizationProvider<T> authorizationProvider,
        ILogger<AuthenticatedUserRequirementHandler<UserHasPermissionRequirement<T>>> logger)
        : base(logger)
    {
        _authorizationProvider = authorizationProvider;
    }

    protected override bool FulfillsRequirement(AuthorizationHandlerContext context, UserHasPermissionRequirement<T> requirement) =>
        _authorizationProvider.HasAnyPermission(requirement.Permissions);
}
