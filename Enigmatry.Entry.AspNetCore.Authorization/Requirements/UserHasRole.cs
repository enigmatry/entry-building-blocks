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

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, Requirement requirement)
        {
            await base.HandleRequirementAsync(context, requirement);

            if (context.HasFailed)
            {
                return;
            }

            if (UserHasRole(context))
            {
                context.Succeed(requirement);
            }
            else
            {
                var requirementName = nameof(Requirement);
                var resource = GetActionAndControllerNames(context);
                Logger.LogWarning("{Requirement} has not been meet for the authorization context for {@Resource}. " +
                                  "This means that user does not have appropriate role.", requirementName, resource);
                context.Fail();
            }
        }

        protected virtual bool UserHasRole(AuthorizationHandlerContext context)
        {
            if (context.Resource is not AuthorizationFilterContext)
            {
                return true;
            }

            var userHasRoleAttribute = TryGetUserHasRoleAttribute(context);
            if (userHasRoleAttribute is not null)
            {
                return _authorizationProvider.HasRole(userHasRoleAttribute.Role);
            }

            return false;
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
