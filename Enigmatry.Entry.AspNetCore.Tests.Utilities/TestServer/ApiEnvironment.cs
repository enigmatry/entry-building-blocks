using Autofac;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.TestServer;

public class ApiEnvironment<T> : WebApplicationFactory<T>, IEnvironment where T : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _ = builder.UseEnvironment("Test");
        _ = builder.ConfigureHostConfiguration(config =>
        {
            _ = config.AddJsonFile("appsettings.Test.json");
        });
        _ = builder.ConfigureContainer<ContainerBuilder>(OnContainerBuilderConfiguration);

        return base.CreateHost(builder);
    }

    public virtual Task<HttpClient> SetupClient()
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            BaseAddress = new Uri("https://localhost")
        });
        client.Timeout = TimeSpan.FromSeconds(30);
        return Task.FromResult(client);
    }

    public virtual void OnContainerBuilderConfiguration(HostBuilderContext context, ContainerBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(builder);
    }
}
