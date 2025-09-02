using Azure.Search.Documents;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AzureSearch;

public class DefaultSearchService<T> : ISearchService<T>
{
    private readonly ISearchClientFactory<T> _searchClientFactory;
    private readonly ILogger<DefaultSearchService<T>> _logger;

    public DefaultSearchService(ISearchClientFactory<T> searchClientFactory, ILogger<DefaultSearchService<T>> logger)
    {
        _searchClientFactory = searchClientFactory;
        _logger = logger;
    }

    public async Task UpdateDocument(T document, CancellationToken cancellationToken = default) =>
        await UpdateDocuments([document], cancellationToken);

    public async Task DeleteDocument(T document, CancellationToken cancellationToken = default) =>
        await DeleteDocuments([document], cancellationToken);

    public async Task UpdateDocuments(IEnumerable<T> documents, CancellationToken cancellationToken = default)
    {
        var client = _searchClientFactory.Create();

        _logger.LogDebug("Uploading documents to index: {IndexName}", client.IndexName);
        await client.UploadDocumentsAsync(documents, cancellationToken: cancellationToken);
    }

    public async Task DeleteDocuments(IEnumerable<T> documents, CancellationToken cancellationToken = default)
    {
        var client = _searchClientFactory.Create();

        _logger.LogDebug("Deleting documents from index: {IndexName}", client.IndexName);
        await client.DeleteDocumentsAsync(documents, cancellationToken: cancellationToken);
    }

    public async Task<SearchResponse<T>> Search(string searchText, SearchOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var escaped = SearchText.AsEscaped(searchText);
        return await Search(escaped, options, cancellationToken);
    }

    public async Task<SearchResponse<T>> Search(SearchText searchText, SearchOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var client = _searchClientFactory.Create();
        _logger.LogDebug("Searching documents in index: {IndexName}", client.IndexName);

        var result = await client.SearchAsync<T>(searchText.Value, options, cancellationToken);
        _logger.LogDebug("Search results. TotalCount: {TotalCount}, ", result.Value.TotalCount);

        return new SearchResponse<T>(result.Value);
    }
}
