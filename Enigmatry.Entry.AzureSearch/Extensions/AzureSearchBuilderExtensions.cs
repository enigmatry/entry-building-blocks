using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Indexes;
using Enigmatry.Entry.AzureSearch.Vectors;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch.Extensions;

public static class AzureSearchBuilderExtensions
{
    public static AzureSearchDocumentBuilder<T> AddDocument<T>(this IAzureSearchBuilder builder,
        string? indexName = null, bool vectorSearch = false)
    {
        builder.Services.AddScoped<ISearchIndexNameResolver<T>, DefaultSearchIndexNameResolver<T>>(
            _ => !string.IsNullOrEmpty(indexName) ? new DefaultSearchIndexNameResolver<T>(indexName) : new DefaultSearchIndexNameResolver<T>());
        builder.Services.AddScoped<ISearchIndexBuilder<T>, DefaultSearchIndexBuilder<T>>();
        builder.Services.AddScoped<ISearchClientFactory<T>, DefaultSearchClientFactory<T>>();
        builder.Services.AddScoped<ISearchIndexManager<T>, DefaultSearchIndexManager<T>>();

        if (vectorSearch)
        {
            builder.Services.AddScoped<ISearchService<T>, VectorSearchService<T>>();
        }
        else
        {
            builder.Services.AddScoped<ISearchService<T>, DefaultSearchService<T>>();
        }

        builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();

        return new AzureSearchDocumentBuilder<T>(builder.Services);
    }
}
