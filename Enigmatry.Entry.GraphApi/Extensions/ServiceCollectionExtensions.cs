﻿using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

namespace Enigmatry.Entry.GraphApi.Extensions;

public static class ServiceCollectionExtensions
{
    [Obsolete("Use AddEntryGraphApi instead")]
    public static void AppAddGraphApi(this IServiceCollection services, IConfiguration configuration) =>
        services.AddEntryGraphApi(configuration);

    public static void AddEntryGraphApi(this IServiceCollection services, IConfiguration configuration) =>
        services.AddScoped<GraphServiceClient>(provider => CreateGraphServiceClient(configuration));

    private static GraphServiceClient CreateGraphServiceClient(IConfiguration configuration)
    {
        var settings = configuration.ResolveHealthCheckSettings();
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
