using Enigmatry.Entry.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;

// authorization building block targets only NET7 
#if NET7_0_OR_GREATER
public static class AuthorizationStartupExtensions
{
    public static void AppAddAuthorization(this IServiceCollection services)
    {
        services.AppAddAuthorization<PermissionId>();
        services.AddScoped<IAuthorizationProvider<PermissionId>, SampleAuthorizationProvider>();
    }
}
#endif
