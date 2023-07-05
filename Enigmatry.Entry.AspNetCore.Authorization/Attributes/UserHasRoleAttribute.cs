using Enigmatry.Entry.AspNetCore.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;

#pragma warning disable CA1813 // Avoid unsealed attributes
public class UserHasRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public const string PolicyPrefix = "UserHasRole";

    /// <summary>
    /// Initializes a new instance of the <see cref="UserHasRoleAttribute"/> class.
    /// </summary>
    /// <param name="roles">A comma delimited list of roles that are allowed to access the resource.</param>
    public UserHasRoleAttribute(params string[] roles)
        : base(PolicyNames.Format(PolicyPrefix, roles)) { }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.Result is ForbidResult or ChallengeResult)
        {
            var logger = context.HttpContext.Resolve<ILogger<UserHasRoleAttribute>>();
            logger.LogWarning($"Forbidden access. Uri: {context.HttpContext.Request.GetDisplayUrl()}");
        }
    }
}
