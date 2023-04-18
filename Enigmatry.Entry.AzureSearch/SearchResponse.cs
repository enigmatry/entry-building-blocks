using Azure;
using Azure.Search.Documents.Models;

namespace Enigmatry.Entry.AzureSearch;

public class SearchResponse<T>
{
    public SearchResponse(Pageable<SearchResult<T>> pagedResult, long? totalCount)
    {
        PagedResult = pagedResult;
        TotalCount = totalCount;
    }

    public long? TotalCount { get; }

    public Pageable<SearchResult<T>> PagedResult { get; }
}
