using Enigmatry.Entry.AspNetCore.Filters;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;
public sealed class UserHasRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserHasRoleAttribute"/> class.
    /// </summary>
    /// <param name="roles">A comma delimited list of roles that are allowed to access the resource.</param>
    public UserHasRoleAttribute(string roles) : base(PolicyNames.UserHasRole)
    {
        Roles = roles;
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
