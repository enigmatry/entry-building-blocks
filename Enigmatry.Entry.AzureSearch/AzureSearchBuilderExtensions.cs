using System;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch;

public static class AzureSearchBuilderExtensions
{
    public static AzureSearchDocumentBuilder<T> AddDocument<T>(this IAzureSearchBuilder builder, string? indexName = null)
    {
        builder.Services.AddScoped<ISearchIndexNameResolver<T>, DefaultSearchIndexNameResolver<T>>(
            _ => !String.IsNullOrEmpty(indexName) ? new DefaultSearchIndexNameResolver<T>(indexName) : new DefaultSearchIndexNameResolver<T>());
        builder.Services.AddScoped<ISearchIndexFactory<T>, DefaultSearchIndexFactory<T>>();
        builder.Services.AddScoped<ISearchClientFactory<T>, DefaultSearchClientFactory<T>>();
        builder.Services.AddScoped<ISearchIndexManager<T>, DefaultSearchIndexManager<T>>();
        builder.Services.AddScoped<ISearchService<T>, DefaultSearchService<T>>();

        return new AzureSearchDocumentBuilder<T>(builder.Services);
    }
}
