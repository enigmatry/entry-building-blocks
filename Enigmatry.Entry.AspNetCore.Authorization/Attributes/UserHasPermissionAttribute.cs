using Enigmatry.Entry.AspNetCore.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;

#pragma warning disable CA1813 // Avoid unsealed attributes 
public class UserHasPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public const string PolicyPrefix = "UserHasPermission";

    /// <summary>
    /// Initializes a new instance of the <see cref="UserHasPermissionAttribute"/> class.
    /// </summary>
    /// <param name="permissions">A comma delimited list of permissions that are required to access the resource.</param>
    public UserHasPermissionAttribute(params string[] permissions)
        : base(PolicyNames.Format(PolicyPrefix, permissions)) { }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.Result is ForbidResult or ChallengeResult)
        {
            var logger = context.HttpContext.Resolve<ILogger<UserHasPermissionAttribute>>();
            logger.LogWarning($"Forbidden access. Uri: {context.HttpContext.Request.GetDisplayUrl()}");
        }
    }
}
