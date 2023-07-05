using Enigmatry.Entry.AspNetCore.Authorization.Attributes;
using Enigmatry.Entry.AspNetCore.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Authorization;

internal class RolesAndPermissionsPolicyProvider : IAuthorizationPolicyProvider
{
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(UserHasRoleAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var requirement =
                new UserHasRoleRequirement(PolicyNames.Parse(UserHasRoleAttribute.PolicyPrefix, policyName));
            return Task.FromResult(new AuthorizationPolicyBuilder().AddRequirements(requirement).Build())!;
        }

        if (policyName.StartsWith(UserHasPermissionAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var requirement =
                new UserHasPermissionRequirement(PolicyNames.Parse(UserHasPermissionAttribute.PolicyPrefix, policyName));
            return Task.FromResult(new AuthorizationPolicyBuilder().AddRequirements(requirement).Build())!;
        }

        return GetDefaultPolicyAsync()!;
    }

    // TODO:This also adds the policy to swagger and healthcheck endpoints. Need to find a solution
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => GetDefaultPolicyAsync()!;
}
