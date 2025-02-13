﻿using Azure.Identity;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

namespace Enigmatry.Entry.GraphApi.Extensions;

[UsedImplicitly]
public static class ServiceCollectionExtensions
{
    [PublicAPI]
    [Obsolete("Use BaseBearerModule or BaseClientSecretModule instead")]
    public static void AddEntryGraphApi(this IServiceCollection services, IConfiguration configuration) =>
        services.AddScoped(_ => CreateGraphServiceClient(configuration));

    [PublicAPI]
    [Obsolete("Use BaseBearerModule or BaseClientSecretModule instead")]
    public static void AddEntryGraphApi(this IServiceCollection services) =>
        services.AddScoped(_ =>
        {
            var managedIdentityCredential = new ManagedIdentityCredential();
            return new GraphServiceClient(managedIdentityCredential);
        });

    private static GraphServiceClient CreateGraphServiceClient(IConfiguration configuration)
    {
        var settings = configuration.ResolveGraphApiSettings();
        var scopes =
            new[]
            {
                "https://graph.microsoft.com/.default"
            }; // Client credential flows must have a scope value with /.default
        var clientSecretCredential =
            new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);

        return new GraphServiceClient(clientSecretCredential, scopes);
    }
}
