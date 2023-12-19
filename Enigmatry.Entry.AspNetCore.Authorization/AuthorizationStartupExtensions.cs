
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
    /// Registers authorization services for the chosen permission type and enables permission-based authorization.
    /// Permission type can be <see cref="string" />, <see cref="Enum" />, <see cref="int" /> or any other type that can be converted from and to string using  <see cref="System.ComponentModel.TypeConverter" />.
    /// Underneath the covers, it uses policy based authentication and permissions are encoded to a policy name.
    /// Because of this, it is necessary that the permission type is capable of converting to and from a string.
    /// </summary>
    /// <typeparam name="TPermission">Permission type</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <exception cref="ArgumentException">Thrown when the TPermission type can not be converted to or from string. You need to implement <see cref="System.ComponentModel.TypeConverter" /> when using custom permission type.</exception>
    /// <returns>The <see cref="AuthorizationBuilder"/> so that additional calls can be chained.</returns>
    public static AuthorizationBuilder AppAddAuthorization<TPermission>(this IServiceCollection services) where TPermission : notnull
    {
        PermissionTypeConverter<TPermission>.EnsureConversionToPolicyNameIsPossible();

        services.AddScoped<IAuthorizationHandler, UserHasPermissionRequirementHandler<TPermission>>();
        services.AddSingleton<IAuthorizationPolicyProvider, UserHasPermissionPolicyProvider<TPermission>>();

        return services.AddAuthorizationBuilder();
    }
}
