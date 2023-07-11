using Enigmatry.Entry.AspNetCore.Authorization.Requirements;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Authorization;

[PublicAPI]
public static class AuthorizationStartupExtensions
{
    public static ControllerActionEndpointConventionBuilder AppRequireAuthorization(
        this ControllerActionEndpointConventionBuilder builder,
        bool enable) =>
        !enable ? builder : builder.RequireAuthorization();

    public static AuthorizationBuilder AppAddAuthorization<T>(this IServiceCollection services, bool enable = true)
    {
        if (enable)
        {
            services.AddScoped<IAuthorizationHandler, UserHasPermissionRequirementHandler<T>>();
            services.AddSingleton<IAuthorizationPolicyProvider, UserHasPermissionPolicyProvider<T>>();
        }

        return services.AddAuthorizationBuilder();
    }
}
