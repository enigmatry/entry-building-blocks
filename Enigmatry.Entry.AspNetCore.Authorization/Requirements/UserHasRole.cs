using Enigmatry.Entry.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

public static class UserHasRole
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

            var userHasRoleAttribute = TryGetUserHasRoleAttribute(context);
            return userHasRoleAttribute is not null && _authorizationProvider.HasAnyRole(userHasRoleAttribute.Roles!.Split(','));
        }

        protected static UserHasRoleAttribute? TryGetUserHasRoleAttribute(AuthorizationHandlerContext context)
        {
            if (context.Resource is AuthorizationFilterContext authContext)
            {
                var userHasRoleAttribute = authContext.Filters.SingleOrDefault(x => x is UserHasRoleAttribute);
                return userHasRoleAttribute == null
                    ? null
                    : (UserHasRoleAttribute)userHasRoleAttribute;
            }
            return null;
        }
    }
}
