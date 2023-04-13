using Enigmatry.Entry.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public static class UserHasPermission
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    public class Requirement : IAuthorizationRequirement { }

    public class RequirementHandler : AuthenticatedUserRequirementHandler<Requirement>
    {
        private readonly IAuthorizationProvider _authorizationProvider;

        public RequirementHandler(
            IAuthorizationProvider authorizationProvider,
            ILogger<AuthenticatedUserRequirementHandler<Requirement>> logger)
            : base(logger)
        {
            _authorizationProvider = authorizationProvider;
        }

        protected override bool FulfillsRequirement(AuthorizationHandlerContext context)
        {
            if (context.Resource is not AuthorizationFilterContext)
            {
                return true;
            }

            var userHasPermissionAttribute = TryGetUserHasPermissionAttribute(context);

            return userHasPermissionAttribute is not null && _authorizationProvider.HasAnyPermission(userHasPermissionAttribute.Permissions.Split(','));
        }

        protected static UserHasPermissionAttribute? TryGetUserHasPermissionAttribute(AuthorizationHandlerContext context)
        {
            if (context.Resource is AuthorizationFilterContext authContext)
            {
                var userHasPermissionAttribute = authContext.Filters.SingleOrDefault(x => x is UserHasPermissionAttribute);
                return userHasPermissionAttribute == null
                    ? null
                    : (UserHasPermissionAttribute)userHasPermissionAttribute;
            }
            return null;
        }
    }
}
