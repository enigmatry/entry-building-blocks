

using Autofac;
using Microsoft.Extensions.Hosting;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.TestServer;

public interface IEnvironment
{
    Task<HttpClient> SetupClient();
    void OnContainerBuilderConfiguration(HostBuilderContext context, ContainerBuilder builder);
}
