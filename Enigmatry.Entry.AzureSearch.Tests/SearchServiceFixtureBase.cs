using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch.Tests;

public abstract class SearchServiceFixtureBase
{
    private ServiceProvider _services = null!;
    private ISearchIndexManager<TestDocument> _indexManager = null!;
    private ISearchService<TestDocument> _searchService = null!;

    [SetUp]
    public async Task Setup()
    {
        _services = new ServiceCollectionBuilder().Build();

        _searchService = _services.GetRequiredService<ISearchService<TestDocument>>();
        _indexManager = _services.GetRequiredService<ISearchIndexManager<TestDocument>>();

        await _indexManager.RecreateIndex();
    }

    protected async Task<SearchResponse<TestDocument>> Search(SearchText searchText,
        SearchOptions? searchOptions = null) => await _searchService.Search(searchText, searchOptions);

    protected async Task<SearchResponse<TestDocument>> Search(string searchText, SearchOptions? searchOptions = null) =>
        await _searchService.Search(searchText, searchOptions);

    protected async Task UpdateDocuments(IEnumerable<TestDocument> documents)
    {
        await _searchService.UpdateDocuments(documents);
        WaitIndexToBeUpdated();
    }

    private static void WaitIndexToBeUpdated() => Thread.Sleep(TimeSpan.FromSeconds(2));

    protected static VerifySettings CreateVerifySettings()
    {
        var settings = new VerifySettings();
        // not all properties were displayed when serializing FacetResult so custom converter was needed
        settings.AddExtraSettings(_ => _.Converters.Add(new FacetResultJsonConverter()));
        settings.IgnoreMember<SearchResult<TestDocument>>(_ => _.Score);//score is not always the same
        return settings;
    }
}
