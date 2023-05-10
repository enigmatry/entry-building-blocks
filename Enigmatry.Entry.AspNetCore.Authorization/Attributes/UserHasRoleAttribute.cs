using Enigmatry.Entry.AspNetCore.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;
public sealed class UserHasRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private const string PolicyPrefix = "UserHasRole";

    public new string[] Roles
    {
        get => Policy != null ? Policy[PolicyPrefix.Length..].Split(',') : Array.Empty<string>();
        private init => Policy = $"{PolicyPrefix}{String.Join(',', value)}";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserHasRoleAttribute"/> class.
    /// </summary>
    /// <param name="roles">A comma delimited list of roles that are allowed to access the resource.</param>
    public UserHasRoleAttribute(string roles) : base(PolicyNames.UserHasRole)
    {
        Roles = roles.Split(',');
    }


    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.Result is ForbidResult or ChallengeResult)
        {
            var logger = context.HttpContext.Resolve<ILogger<UserHasRoleAttribute>>();
            logger.LogWarning($"Forbidden access. Uri: {context.HttpContext.Request.GetDisplayUrl()}");
        }
    }
}
