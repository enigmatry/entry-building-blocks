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

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, Requirement requirement)
        {
            await base.HandleRequirementAsync(context, requirement);

            if (context.HasFailed)
            {
                return;
            }

            if (UserHasPermission(context))
            {
                context.Succeed(requirement);
            }
            else
            {
                var requirementName = nameof(Requirement);
                var resource = GetActionAndControllerNames(context);
                Logger.LogWarning("{Requirement} has not been meet for the authorization context for {@Resource}. " +
                                  "This means that user does not have appropriate permissions.", requirementName, resource);
                context.Fail();
            }
        }

        protected virtual bool UserHasPermission(AuthorizationHandlerContext context)
        {
            if (context.Resource is not AuthorizationFilterContext)
            {
                return true;
            }

            var userHasPermissionAttribute = TryGetUserHasPermissionAttribute(context);

            //dynamically get required permissions
            //var something = GetActionAndControllerNames(context);

            if (userHasPermissionAttribute is { Action: { }, EntityType: { } })
            {
                return _authorizationProvider.HasPermission(userHasPermissionAttribute.Action, userHasPermissionAttribute.EntityType);
            }

            return false;
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
