using Azure.Search.Documents;

namespace Enigmatry.Entry.AzureSearch.Abstractions;

public interface ISearchService<T>
{
    public Task UpdateDocument(T document, CancellationToken cancellationToken = default);
    public Task DeleteDocument(T document, CancellationToken cancellationToken = default);
    public Task UpdateDocuments(IEnumerable<T> documents, CancellationToken cancellationToken = default);
    public Task DeleteDocuments(IEnumerable<T> documents, CancellationToken cancellationToken = default);

    public Task<SearchResponse<T>> Search(SearchText searchText, SearchOptions? options = null, CancellationToken cancellationToken = default);
}
