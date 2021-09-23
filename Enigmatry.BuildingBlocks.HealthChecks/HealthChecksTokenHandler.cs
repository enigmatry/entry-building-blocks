using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Enigmatry.BuildingBlocks.HealthChecks
{
    internal class HealthChecksTokenHandler : AuthorizationHandler<HealthChecksTokenRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HealthChecksTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HealthChecksTokenRequirement requirement)
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Query["token"].ToString() == requirement.Token)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
