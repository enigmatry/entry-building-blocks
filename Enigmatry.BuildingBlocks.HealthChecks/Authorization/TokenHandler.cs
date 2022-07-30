using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.HealthChecks.Authorization
{
    public class TokenHandler : AuthorizationHandler<TokenRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenRequirement requirement)
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Query["token"].ToString() == requirement.Token)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
