using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Search.Documents.Models;
using JetBrains.Annotations;

namespace Enigmatry.Entry.AzureSearch;

[PublicAPI]
public class SearchResponse<T>
{
    public SearchResponse(SearchResults<T> result)
    {
        PagedResult = result.GetResults();
        Facets = result.Facets;
        TotalCount = result.TotalCount;
    }

    public Pageable<SearchResult<T>> PagedResult { get; }

    public IDictionary<string, IList<FacetResult>> Facets { get; }

    public long? TotalCount { get; }
}
