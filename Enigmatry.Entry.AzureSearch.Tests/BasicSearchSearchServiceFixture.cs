using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch.Tests;

[Category("unit")]
public class BasicSearchSearchServiceFixture
{
    private ServiceProvider _services = null!;
    private ISearchIndexManager<TestDocument> _indexManager = null!;
    private ISearchService<TestDocument> _searchService = null!;
    private TestDocument _document = null!;
    private TestDocument _document2 = null!;

    [SetUp]
    public async Task Setup()
    {
        _services = new ServiceCollectionBuilder().Build();

        _searchService = _services.GetRequiredService<ISearchService<TestDocument>>();
        _indexManager = _services.GetRequiredService<ISearchIndexManager<TestDocument>>();

        await _indexManager.RecreateIndex();

        _document = ADocument();
        _document2 = AnotherDocument();

        await _searchService.UpdateDocuments(new[] { _document, _document2 });

        WaitIndexToBeUpdated();
    }

    [Test]
    public async Task TestSearchById()
    {
        var searchResult = await _searchService.Search(_document.Id);
        await Verify(searchResult, CreateVerifySettings());
    }

    [Test]
    public async Task TestSearchByName()
    {
        var searchResult = await _searchService.Search(_document.Name);
        await Verify(searchResult, CreateVerifySettings());
    }

    [Test]
    public async Task TestSearchByIdWithOptions()
    {
        var searchResult = await _searchService.Search(_document.Id, ASearchOptionsWithDefaultSizeAndSkip());
        await Verify(searchResult, CreateVerifySettings());
    }

    [Test]
    public async Task TestSearchByNameWithOptions()
    {
        var searchResult = await _searchService.Search(_document.Name, ASearchOptionsWithDefaultSizeAndSkip());
        await Verify(searchResult, CreateVerifySettings());
    }

    [Test]
    public async Task TestSearchByFilter()
    {
        var searchOptions = new SearchOptions
        {
            Filter = $"{nameof(TestDocument.Name)} eq 'Harry Potter'",
            OrderBy = { "Name" },
            HighlightFields = { "Name" },
            Size = 10,
            Skip = 0,
            IncludeTotalCount = true
        };
        var searchResult = await _searchService.Search("", searchOptions);
        await Verify(searchResult, CreateVerifySettings());
    }

    [Test]
    public async Task TestSearchByDescription()
    {
        var searchResult = await _searchService.Search(_document.Description);
        await Verify(searchResult, CreateVerifySettings());
    }

    private static TestDocument ADocument() => new TestDocumentBuilder().Build();

    private static TestDocument AnotherDocument() =>
        new TestDocumentBuilder().WithId("23432432").WithName("Harry Potter").WithDescription("Hogwarts")
            .Build();

    private static SearchOptions ASearchOptionsWithDefaultSizeAndSkip() =>
        new() { Size = 10, Skip = 0, IncludeTotalCount = true };

    private static void WaitIndexToBeUpdated() => Thread.Sleep(TimeSpan.FromSeconds(2));


    private static VerifySettings CreateVerifySettings()
    {
        var settings = new VerifySettings();
        settings.IgnoreMember<SearchResult<TestDocument>>(_ => _.Score);//score is not always the same
        return settings;
    }
}
