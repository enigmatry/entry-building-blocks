using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Search.Documents;

namespace Enigmatry.Entry.AzureSearch.Abstractions;

public interface ISearchService<T>
{
    Task UpdateDocument(T document, CancellationToken cancellationToken = default);
    Task DeleteDocument(T document, CancellationToken cancellationToken = default);
    Task UpdateDocuments(IEnumerable<T> documents, CancellationToken cancellationToken = default);
    Task DeleteDocuments(IEnumerable<T> documents, CancellationToken cancellationToken = default);

    Task<SearchResponse<T>> Search(string searchText, SearchOptions? options = null, CancellationToken cancellationToken = default);
}
