﻿using Enigmatry.Entry.AspNetCore.Authorization.Requirements;
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
        if (!enable)
        {
            return services.AddAuthorizationBuilder();
        }

        services.AddScoped<IAuthorizationHandler, UserHasRole.RequirementHandler>();
        services.AddScoped<IAuthorizationHandler, UserHasPermission.RequirementHandler>();

        return services.AddAuthorizationBuilder()
            .AddPolicy(
                PolicyNames.UserHasRole,
                policy => policy.AddRequirements(new UserHasRole.Requirement()))
            .AddPolicy(
                PolicyNames.UserHasPermission,
                policy => policy.AddRequirements(new UserHasPermission.Requirement()))
            // TODO:This also adds the policy to swagger and healthcheck endpoints. Need to find a solution
            .AddFallbackPolicy(
                PolicyNames.UserAuthenticated,
                new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
    }
}
