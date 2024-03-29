﻿using Azure;
using Azure.Search.Documents.Indexes;
using Enigmatry.Entry.AzureSearch.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Enigmatry.Entry.AzureSearch.Extensions;

[PublicAPI]
public static class ServiceCollectionExtensions
{
    public static IAzureSearchBuilder AddAzureSearch(this IServiceCollection services, SearchSettings searchSettings) => services.AddAzureSearch(searchSettings.CopyPropertiesTo);

    public static IAzureSearchBuilder AddAzureSearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SearchSettings>(configuration);

        return services.AddAzureSearch();
    }

    public static IAzureSearchBuilder AddAzureSearch(this IServiceCollection services,
        Action<SearchSettings> searchOptions)
    {
        services.Configure(searchOptions);

        return services.AddAzureSearch();
    }

    public static IAzureSearchBuilder AddAzureSearch(this IServiceCollection services)
    {
        services.AddScoped(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<SearchSettings>>();
            return new SearchIndexClient(options.Value.SearchServiceEndPoint,
                new AzureKeyCredential(options.Value.ApiKey));
        });

        return new AzureSearchBuilder(services);
    }
}
