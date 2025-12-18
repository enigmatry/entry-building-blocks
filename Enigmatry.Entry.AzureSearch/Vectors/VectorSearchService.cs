using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AzureSearch.Vectors;

public class VectorSearchService<T> : DefaultSearchService<T>
{
    private readonly IEmbeddingService _embeddingService;

    public VectorSearchService(ISearchClientFactory<T> searchClientFactory,
        IEmbeddingService embeddingService,
        ILogger<DefaultSearchService<T>> logger) : base(searchClientFactory, logger)
    {
        _embeddingService = embeddingService;
    }

    public async Task UpdateDocument(T document, IDictionary<string, string> vectorFields,
        CancellationToken cancellationToken = default)
    {
        await vectorFields.ForEach(async vectorField =>
        {
            var stringProperty = typeof(T).GetProperty(vectorField.Key);
            var rawValue = stringProperty?.GetValue(document)?.ToString() ?? string.Empty;
            var vectorProperty = typeof(T).GetProperty(vectorField.Value);
            vectorProperty?.SetValue(document, await _embeddingService.EmbedText(rawValue));
        });

        await base.UpdateDocument(document, cancellationToken);
    }

    public async Task<SearchResponse<T>> SingleVectorSearch(SearchText searchText,
        IEnumerable<string> vectorFields, SearchOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await PrepareVector(searchText, vectorFields, options);
        return await Search(searchText, options, cancellationToken);
    }

    private async Task PrepareVector(SearchText searchText, IEnumerable<string> vectorFields, SearchOptions? options = null)
    {
        options ??= new SearchOptions();
        var vectorEmbedding = await _embeddingService.EmbedText(searchText.Value);
        var query = new VectorizedQuery(vectorEmbedding);
        vectorFields.ToList().ForEach(query.Fields.Add);
        var vectorSearchOptions = new VectorSearchOptions();
        vectorSearchOptions.Queries.Add(query);
        options.VectorSearch = vectorSearchOptions;
    }
}
