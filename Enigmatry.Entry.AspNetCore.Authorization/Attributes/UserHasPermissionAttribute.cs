using Enigmatry.Entry.AspNetCore.Filters;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;
public sealed class UserHasPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public string Permission { get; }

    public UserHasPermissionAttribute(string permission) : base(PolicyNames.UserHasPermission)
    {
        Permission = permission;
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
