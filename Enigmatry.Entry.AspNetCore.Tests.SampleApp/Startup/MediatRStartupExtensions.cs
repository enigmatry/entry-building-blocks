using System.Reflection;
using Enigmatry.Entry.MediatR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Startup;

public static class MediatRStartupExtensions
{
    public static void AppAddMediatR(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assembly);
        });
    }
}
