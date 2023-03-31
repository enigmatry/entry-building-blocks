using Enigmatry.Entry.AspNetCore.Filters;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;
public sealed class UserHasPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public string? EntityType { get; }
    public string? Action { get; }

    public UserHasPermissionAttribute() : base(PolicyNames.UserHasPermission)
    {
    }

    public UserHasPermissionAttribute(string entityType, string action) : base(PolicyNames.UserHasPermission)
    {
        EntityType = entityType;
        Action = action;
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
