using Enigmatry.Entry.AspNetCore.Authorization.Attributes;
using Enigmatry.Entry.AspNetCore.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Enigmatry.Entry.AspNetCore.Authorization;

internal class UserHasPermissionPolicyProvider<TPermission> : IAuthorizationPolicyProvider where TPermission : notnull
{
    private readonly DefaultAuthorizationPolicyProvider _defaultPolicyProvider;

    public UserHasPermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _defaultPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(UserHasPermissionAttribute<TPermission>.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            // Caching could be added here; AuthorizationPolicy can be created once per policyName and cached.
            var requirement =
                new UserHasPermissionRequirement<TPermission>(
                    PermissionTypeConverter<TPermission>.ConvertFromPolicyName(UserHasPermissionAttribute<TPermission>.PolicyPrefix, policyName));
            return Task.FromResult(new AuthorizationPolicyBuilder().AddRequirements(requirement).Build())!;
        }

        return _defaultPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _defaultPolicyProvider.GetDefaultPolicyAsync(); // DefaultPolicy is RequireAuthenticatedUser

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _defaultPolicyProvider.GetFallbackPolicyAsync();
}
