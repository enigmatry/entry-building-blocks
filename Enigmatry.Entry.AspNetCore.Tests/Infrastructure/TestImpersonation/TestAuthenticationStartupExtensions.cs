using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Tests.Infrastructure.TestImpersonation;

public static class TestAuthenticationStartupExtensions
{
    public static AuthenticationBuilder AddTestUserAuthentication(this IServiceCollection services) =>
        services.AddAuthentication(TestUserAuthenticationHandler.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, TestUserAuthenticationHandler>(
                TestUserAuthenticationHandler.AuthenticationScheme, _ => { });


    public static AuthenticationBuilder AddNoResultAuthentication(this IServiceCollection services) =>
        services.AddAuthentication(NoResultAuthenticationHandler.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, NoResultAuthenticationHandler>(
                NoResultAuthenticationHandler.AuthenticationScheme, _ => { });

}
