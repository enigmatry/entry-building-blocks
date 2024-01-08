using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Enigmatry.Entry.MediatR.Tests;

public class TestServiceProviderBuilder
{
    private readonly Assembly _testAssembly = typeof(TestServiceProviderBuilder).Assembly;

    public IServiceProvider Build()
    {
        var serviceCollection = new ServiceCollection();
        AppAddLogger(serviceCollection);
        AppAddFluentValidation(serviceCollection);
        AppAddMediatR(serviceCollection);

        return serviceCollection.BuildServiceProvider();
    }

    private static void AppAddLogger(IServiceCollection serviceCollection) => serviceCollection.AddLogging();

    private void AppAddMediatR(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        serviceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        serviceCollection.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(_testAssembly);
        });
    }

    private void AppAddFluentValidation(IServiceCollection serviceCollection) =>
        serviceCollection.AddValidatorsFromAssembly(_testAssembly);
}
