using Enigmatry.Entry.AspNetCore.Authorization.Attributes;
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

    /// <summary>
    /// Enables permission based authorization and registers authorization services for the specified permission type.
    /// </summary>
    /// <typeparam name="TPermission">Permission type</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="AuthorizationBuilder"/> so that additional calls can be chained.</returns>
    public static AuthorizationBuilder AppAddAuthorization<TPermission>(this IServiceCollection services) where TPermission : notnull
    {
        PermissionTypeConverter<TPermission>.EnsureConversionToPolicyNameIsPossible();

        services.AddScoped<IAuthorizationHandler, UserHasPermissionRequirementHandler<TPermission>>();
        services.AddSingleton<IAuthorizationPolicyProvider, UserHasPermissionPolicyProvider<TPermission>>();

        return services.AddAuthorizationBuilder();
    }
}
