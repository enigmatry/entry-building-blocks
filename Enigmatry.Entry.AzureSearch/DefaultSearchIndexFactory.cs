using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AzureSearch;

public class DefaultSearchIndexFactory<T> : ISearchIndexFactory<T>
{
    private readonly ISearchIndexNameResolver<T> _indexNameResolver;
    private readonly ILogger<DefaultSearchIndexFactory<T>> _logger;

    public DefaultSearchIndexFactory(ISearchIndexNameResolver<T> indexNameResolver, ILogger<DefaultSearchIndexFactory<T>> logger)
    {
        _indexNameResolver = indexNameResolver;
        _logger = logger;
    }

    public virtual SearchIndex Create()
    {
        FieldBuilder fieldBuilder = new();
        var searchFields = fieldBuilder.Build(typeof(T));

        var indexName = _indexNameResolver.ResolveIndexName();

        _logger.LogInformation("Creating index: {IndexName}", indexName);

        return new SearchIndex(indexName, searchFields);
    }
}
