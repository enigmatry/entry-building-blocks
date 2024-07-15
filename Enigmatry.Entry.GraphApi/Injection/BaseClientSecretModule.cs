using Autofac;
using Azure.Identity;
using Enigmatry.Entry.Core.Settings;
using JetBrains.Annotations;
using Microsoft.Graph;

namespace Enigmatry.Entry.GraphApi.Injection;

[PublicAPI]
public abstract class BaseClientSecretModule<T> : Module where T : class
{
    protected abstract void RegisterManagementServices(ContainerBuilder builder);
    protected abstract T ResolveManagementServiceFrom(IComponentContext context, GraphApiSettings options);

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(CreateGraphServiceClient).As<GraphServiceClient>().InstancePerLifetimeScope();

        RegisterManagementServices(builder);

        builder.Register(CreateGraphManagementService).As<T>().InstancePerLifetimeScope();
    }

    private static GraphServiceClient CreateGraphServiceClient(IComponentContext c)
    {
        var options = c.Resolve<GraphApiSettings>();

        var scopes = new[] { "https://graph.microsoft.com/.default" }; // Client credential flows must have a scope value with /.default

        var clientId = options.ClientId;
        var clientSecret = options.ClientSecret;

        var clientSecretCredential = new ClientSecretCredential(options.TenantId, clientId, clientSecret);

        return new GraphServiceClient(clientSecretCredential, scopes);
    }

    private T CreateGraphManagementService(IComponentContext context)
    {
        var options = context.Resolve<GraphApiSettings>();
        return ResolveManagementServiceFrom(context, options);
    }
}
