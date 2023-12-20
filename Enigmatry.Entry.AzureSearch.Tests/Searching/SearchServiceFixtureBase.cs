using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch.Tests.Searching;

public abstract class SearchServiceFixtureBase
{
    private ServiceProvider _services = null!;
    private ISearchIndexManager<TestDocument> _indexManager = null!;
    private ISearchService<TestDocument> _searchService = null!;

    private static readonly TimeSpan DefaultIndexRecreateWait = TimeSpan.FromSeconds(1);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _services = new ServiceCollectionBuilder().Build();

        _searchService = _services.GetRequiredService<ISearchService<TestDocument>>();
        _indexManager = _services.GetRequiredService<ISearchIndexManager<TestDocument>>();

        await _indexManager.RecreateIndex();
        WaitIndexToBeUpdated();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => _services.Dispose();

    [SetUp]
    public async Task Setup() => await DeleteAllDocuments(); // remove all leftovers from previous test run

    private async Task DeleteAllDocuments()
    {
        var searchResponse = await _searchService.Search(SearchText.AsNotEscaped("*"));
        var documents = searchResponse.PagedResult.AsPages().First().Values.Select(x => x.Document).ToList();
        if (documents.Any())
        {
            await _searchService.DeleteDocuments(documents);
            WaitIndexToBeUpdated();
        }
    }

    protected async Task<SearchResponse<TestDocument>> Search(SearchText searchText,
        SearchOptions? searchOptions = null) => await _searchService.Search(searchText, searchOptions);

    protected async Task<SearchResponse<TestDocument>> Search(string searchText, SearchOptions? searchOptions = null) =>
        await _searchService.Search(SearchText.AsEscaped(searchText), searchOptions);

    protected async Task UpdateDocuments(IEnumerable<TestDocument> documents) =>
        await UpdateDocuments(documents, DefaultIndexRecreateWait);

    protected async Task UpdateDocuments(IEnumerable<TestDocument> documents, TimeSpan waitTimeSpan)
    {
        await _searchService.UpdateDocuments(documents);
        WaitIndexToBeUpdated(waitTimeSpan);
    }

    private static void WaitIndexToBeUpdated() => WaitIndexToBeUpdated(DefaultIndexRecreateWait);

    private static void WaitIndexToBeUpdated(TimeSpan waitTimeSpan) => Thread.Sleep(waitTimeSpan);

    protected static VerifySettings CreateVerifySettings()
    {
        var settings = new VerifySettings();
        // not all properties were displayed when serializing FacetResult so custom converter was needed
        settings.AddExtraSettings(serializerSettings => serializerSettings.Converters.Add(new FacetResultJsonConverter()));
        settings.IgnoreMember<SearchResult<TestDocument>>(_ => _.Score); //score is not always the same
        return settings;
    }
}
