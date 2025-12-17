using Enigmatry.Entry.AspNetCore.OpenTelemetry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal abstract class AuthenticatedUserRequirementHandler<TRequirement>(
    ILogger<AuthenticatedUserRequirementHandler<TRequirement>> logger)
    : AuthorizationHandler<TRequirement>
    where TRequirement : IAuthorizationRequirement
{
    protected readonly ILogger<AuthenticatedUserRequirementHandler<TRequirement>> _logger = logger;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            _logger.LogSecurityWarning("User is not authenticated.");
            context.Fail();
            return Task.CompletedTask;
        }

        if (FulfillsRequirement(context, requirement))
        {
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogSecurityWarning("{Requirement} has not been met for the resource path: {ResourcePath}.", requirement.ToString(), GetResourcePath(context));
            context.Fail();
        }
        return Task.CompletedTask;
    }

    protected abstract bool FulfillsRequirement(AuthorizationHandlerContext context, TRequirement requirement);

    protected static string? GetResourcePath(AuthorizationHandlerContext context) => (context.Resource as HttpContext)?.Request.Path;
}
