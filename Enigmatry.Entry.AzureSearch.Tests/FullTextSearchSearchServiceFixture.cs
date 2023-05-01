using Azure.Search.Documents.Models;
using FluentAssertions;
using static Enigmatry.Entry.AzureSearch.Tests.AzureSearchTestCases;

namespace Enigmatry.Entry.AzureSearch.Tests;

[Category("unit")]
public class FullTextSearchSearchServiceFixture : SearchServiceFixtureBase
{
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

        var searchResult = await Search(searchText, searchOptions);
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
}


