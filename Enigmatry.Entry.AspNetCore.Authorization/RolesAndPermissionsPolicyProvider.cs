using Enigmatry.Entry.AspNetCore.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Authorization;

internal class RolesAndPermissionsPolicyProvider : IAuthorizationPolicyProvider
{
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(UserHasRoleRequirement.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var requirement = new UserHasRoleRequirement(policyName[UserHasRoleRequirement.PolicyPrefix.Length..]);
            return Task.FromResult(new AuthorizationPolicyBuilder().AddRequirements(requirement).Build())!;
        }

        if (policyName.StartsWith(UserHasPermissionRequirement.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var requirement = new UserHasPermissionRequirement(policyName[UserHasPermissionRequirement.PolicyPrefix.Length..]);
            return Task.FromResult(new AuthorizationPolicyBuilder().AddRequirements(requirement).Build())!;
        }

        return GetDefaultPolicyAsync()!;
    }

    // TODO:This also adds the policy to swagger and healthcheck endpoints. Need to find a solution
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => GetDefaultPolicyAsync()!;
}
