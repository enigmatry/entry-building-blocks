using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Setup;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

using static Enigmatry.Entry.AzureSearch.Tests.AzureSearchTestCases;

namespace Enigmatry.Entry.AzureSearch.Tests;

[Category("unit")]
public class FullTextSearchSearchServiceFixture
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

    [TestCaseSource(typeof(AzureSearchTestCases), nameof(AzureSearchSpecialCharactersTestCases))]
    public async Task TestSpecialCharactersSearch(AzureSearchTestCase testCase) => await TestSearch(testCase);

    [TestCaseSource(typeof(AzureSearchTestCases), nameof(AzureSearchFullSearchTestCases))]
    public async Task TestFullSearch(AzureSearchTestCase testCase) => await TestSearch(testCase);

    [TestCaseSource(typeof(AzureSearchTestCases), nameof(AzureSearchPhraseSearchTestCases))]
    public async Task TestPhraseSearch(AzureSearchTestCase testCase) => await TestSearch(testCase);

    private async Task TestSearch(AzureSearchTestCase testCase)
    {
        await UpdateDocuments(testCase.Documents);

        var searchText = testCase.SearchText;
        var searchOptions = testCase.Options;

        var searchResult = await _searchService.Search(searchText, searchOptions);
        var pages = searchResult.PagedResult.AsPages().ToList();

        TestContext.WriteLine("Search text: " + searchText.Value);

        var expectation = testCase.Expectation;
        searchResult.TotalCount.Should().Be(expectation.TotalCount, expectation.Reason);
        pages.Count.Should().Be(expectation.PagesTotalCount);
        if (testCase.Expectation.PagesTotalCount > 0)
        {
            var documentsOnFirstPage = pages.First().Values.Select(r => r.Document).ToList();
            documentsOnFirstPage.Should().BeEquivalentTo(expectation.ExpectedDocumentsOnFirstPage);
        }
    }

    private async Task UpdateDocuments(IEnumerable<TestDocument> documents)
    {
        await _searchService.UpdateDocuments(documents);
        WaitIndexToBeUpdated();
    }

    private static void WaitIndexToBeUpdated() => Thread.Sleep(TimeSpan.FromSeconds(2));
}
