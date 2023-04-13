using Enigmatry.Entry.AspNetCore.Filters;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;
public sealed class UserHasPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public string Permissions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserHasPermissionAttribute"/> class.
    /// </summary>
    /// <param name="permissions">A comma delimited list of permissions that are required to access the resource.</param>
    public UserHasPermissionAttribute(string permissions) : base(PolicyNames.UserHasPermission)
    {
        Permissions = permissions;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.Result is ForbidResult or ChallengeResult)
        {
            var logger = context.HttpContext.Resolve<ILogger<UserHasPermissionAttribute>>();
            logger.LogWarning($"Forbidden access. Uri: {context.HttpContext.Request.GetUri()}");
        }
    }
}
