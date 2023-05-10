using Enigmatry.Entry.AspNetCore.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Authorization;
public static class AuthorizationStartupExtensions
{
    public static ControllerActionEndpointConventionBuilder AppRequireAuthorization(
        this ControllerActionEndpointConventionBuilder builder,
        bool enable) =>
        !enable ? builder : builder.RequireAuthorization();

    public static AuthorizationBuilder AppAddAuthorization(this IServiceCollection services, bool enable)
    {
        if (enable)
        {
            services.AddScoped<IAuthorizationHandler, UserHasPermissionRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, UserHasRoleRequirementHandler>();

            services.AddSingleton<IAuthorizationPolicyProvider, RolesAndPermissionsPolicyProvider>();
        }

        return services.AddAuthorizationBuilder();
    }
}
