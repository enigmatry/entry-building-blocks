using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;
public abstract class AuthenticatedUserRequirementHandler<TRequirement>
    : AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement
{
    protected ILogger<AuthenticatedUserRequirementHandler<TRequirement>> Logger { get; }

    protected AuthenticatedUserRequirementHandler(
        ILogger<AuthenticatedUserRequirementHandler<TRequirement>> logger)
    {
        Logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            Logger.LogWarning("User not authenticated.");
            context.Fail();
        }

        return Task.CompletedTask;
    }

    protected static (string actionName, string controllerName) GetActionAndControllerNames(AuthorizationHandlerContext context)
    {
        var result = (actionName: String.Empty, controllerName: String.Empty);
        if (context.Resource == null)
        {
            return result;
        }

        if (context.Resource is AuthorizationFilterContext { ActionDescriptor: ControllerActionDescriptor actionDescriptor })
        {
            result.actionName = actionDescriptor.ActionName;
            result.controllerName = actionDescriptor.ControllerName;
        }
        return result;
    }
}
