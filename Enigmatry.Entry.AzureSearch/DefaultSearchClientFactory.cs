using Azure;
using Azure.Search.Documents;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Microsoft.Extensions.Options;

namespace Enigmatry.Entry.AzureSearch;

public class DefaultSearchClientFactory<T> : ISearchClientFactory<T>
{
    private readonly ISearchIndexNameResolver<T> _indexNameResolver;
    private readonly IOptions<SearchSettings> _searchSettings;

    public DefaultSearchClientFactory(ISearchIndexNameResolver<T> indexNameResolver,
        IOptions<SearchSettings> searchSettings)
    {
        _indexNameResolver = indexNameResolver;
        _searchSettings = searchSettings;
    }

    public SearchClient Create() =>
        new(_searchSettings.Value.SearchServiceEndPoint,
            _indexNameResolver.ResolveIndexName(),
            new AzureKeyCredential(_searchSettings.Value.ApiKey));
}
