﻿using Shouldly;
using static Enigmatry.Entry.AzureSearch.Tests.Searching.AzureSearchTestCases;

namespace Enigmatry.Entry.AzureSearch.Tests.Searching;

[Category("integration")]
[Explicit("Flaky: TODO BP-815: Remove Explicit attribute")]
public class FullTextSearchFixture : SearchServiceFixtureBase
{
    [TestCaseSource(typeof(AzureSearchTestCases), nameof(AzureSearchSpecialCharactersTestCases))]
    public async Task TestSpecialCharactersSearch(AzureSearchTestCase testCase) => await TestSearch(testCase);

    [TestCaseSource(typeof(AzureSearchTestCases), nameof(AzureSearchFullSearchTestCases))]
    public async Task TestFullSearch(AzureSearchTestCase testCase) => await TestSearch(testCase);

    [TestCaseSource(typeof(AzureSearchTestCases), nameof(AzureSearchPhraseSearchTestCases))]
    public async Task TestPhraseSearch(AzureSearchTestCase testCase) => await TestSearch(testCase);

    private async Task TestSearch(AzureSearchTestCase testCase)
    {
        await UpdateDocuments(testCase.Documents, TimeSpan.FromSeconds(2));

        var searchText = testCase.SearchText;
        var searchOptions = testCase.Options;

        var searchResult = await Search(searchText, searchOptions);
        var pages = searchResult.PagedResult.AsPages().ToList();

        await TestContext.Out.WriteLineAsync("Search text: " + searchText.Value);

        var expectation = testCase.Expectation;
        searchResult.TotalCount.ShouldBe(expectation.TotalCount, expectation.Reason);
        pages.Count.ShouldBe(expectation.PagesTotalCount);
        if (testCase.Expectation.PagesTotalCount > 0)
        {
            var documentsOnFirstPage = pages.First().Values.Select(r => r.Document).ToList();
            documentsOnFirstPage.ShouldBeEquivalentTo(expectation.ExpectedDocumentsOnFirstPage);
        }
    }
}


