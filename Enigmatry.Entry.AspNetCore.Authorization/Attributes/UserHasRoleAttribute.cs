using Enigmatry.Entry.AspNetCore.Filters;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;
public sealed class UserHasRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public string Role { get; }

    public UserHasRoleAttribute(string role) : base(PolicyNames.UserHasRole)
    {
        Role = role;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.Result is ForbidResult or ChallengeResult)
        {
            var logger = context.HttpContext.Resolve<ILogger<UserHasRoleAttribute>>();
            logger.LogWarning($"Forbidden access. Uri: {context.HttpContext.Request.GetUri()}");
        }
    }
}
