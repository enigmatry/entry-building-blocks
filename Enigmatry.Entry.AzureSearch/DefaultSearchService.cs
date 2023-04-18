using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents.Models;

namespace Enigmatry.Entry.AzureSearch;

public class DefaultSearchService<T> : ISearchService<T>
{
    private readonly ISearchClientFactory<T> _searchClientFactory;


    public DefaultSearchService(ISearchClientFactory<T> searchClientFactory)
    {
        _searchClientFactory = searchClientFactory;
    }

    public async Task UpdateDocument(T document, CancellationToken cancellationToken = default) =>
        await UpdateDocuments(new List<T> { document }, cancellationToken);

    public async Task DeleteDocument(T document, CancellationToken cancellationToken = default) =>
        await DeleteDocuments(new List<T> { document }, cancellationToken);

    public async Task UpdateDocuments(IEnumerable<T> documents, CancellationToken cancellationToken = default)
    {
        var client = _searchClientFactory.Create();

        await client.UploadDocumentsAsync(documents, cancellationToken: cancellationToken);
    }

    public async Task DeleteDocuments(IEnumerable<T> documents, CancellationToken cancellationToken = default)
    {
        var client = _searchClientFactory.Create();

        await client.DeleteDocumentsAsync(documents, cancellationToken: cancellationToken);
    }

    public async Task<SearchResponse<T>> Search(string searchText,
        Azure.Search.Documents.SearchOptions? options = null, CancellationToken cancellationToken = default)
    {
        var client = _searchClientFactory.Create();

        Response<SearchResults<T>>? result = await client.SearchAsync<T>(searchText, options, cancellationToken);

        Pageable<SearchResult<T>>? pagedResult = result.Value.GetResults()!;

        return new SearchResponse<T>(pagedResult, result.Value.TotalCount);
    }
}
