using Autofac;
using JetBrains.Annotations;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using System;

namespace Enigmatry.Entry.GraphApi.Injection;

[PublicAPI]
public abstract class BaseBearerModule : Module
{
    protected abstract string ExtractBearerTokenFrom(IComponentContext context);
    protected override void Load(ContainerBuilder builder) =>
        builder.Register(ResolveGraphServiceClient).As<GraphServiceClient>().InstancePerLifetimeScope();

    private static GraphServiceClient CreateAuthenticatedClientForUser(string userToken)
    {
        const string baseUrl = "https://graph.microsoft.com/v1.0";

        var authenticationProvider = new BaseBearerTokenAuthenticationProvider(new FixedTokenProvider(userToken));
        var graphClient = new GraphServiceClient(authenticationProvider, baseUrl);
        return graphClient;
    }

    private GraphServiceClient ResolveGraphServiceClient(IComponentContext componentContext)
    {
        var accessToken = ExtractBearerTokenFrom(componentContext);
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new InvalidOperationException("Unable to create graph api client since the user's graph api token wasn't found in the request headers");
        }
        return CreateAuthenticatedClientForUser(accessToken);
    }
}
