using Enigmatry.Entry.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;

public static class AuthorizationStartupExtensions
{
    public static void AppAddAuthorization(this IServiceCollection services)
    {
        services.AddEntryAuthorization<PermissionId>();
        services.AddScoped<IAuthorizationProvider<PermissionId>, SampleAuthorizationProvider>();
    }
}
