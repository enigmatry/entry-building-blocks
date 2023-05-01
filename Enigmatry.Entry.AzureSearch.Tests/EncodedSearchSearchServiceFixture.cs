using Azure.Search.Documents;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Setup;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Azure.Search.Documents.Models;
using static Enigmatry.Entry.AzureSearch.Tests.AzureSearchTestCases;

namespace Enigmatry.Entry.AzureSearch.Tests;

[Category("unit")]
public class EncodedSearchSearchServiceFixture
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

        var documentsWithSpecialCharacters =
            AzureSearchSpecialCharacters.Select((character, index) =>
                ADocumentWith(index.ToString(CultureInfo.InvariantCulture), character));

        await _searchService.UpdateDocuments(documentsWithSpecialCharacters);

        await _searchService.UpdateDocument(new TestDocumentBuilder()
            .WithId("2000")
            .WithDescription("baba i deda")
            .WithName("zaba").Build());

        await _searchService.UpdateDocument(new TestDocumentBuilder()
            .WithId("2001")
            .WithDescription("baba i")
            .WithName("zaba").Build());

        WaitIndexToBeUpdated();
    }

    [TestCaseSource(typeof(AzureSearchTestCases), nameof(AzureSearchSpecialCharactersTestCases))]
    public async Task TestSearchWithSpecialCharactersWithEncoding(AzureSearchTestCase testCase)
    {
        var searchValue = $"FirstPart {testCase.Value} SecondPart";
        var searchText = SearchText.AsPhraseSearch(searchValue);
        var searchOptions =
            new SearchOptions { IncludeTotalCount = true, Size = 10, Skip = 0, SearchMode = SearchMode.All };
        var searchResult = await _searchService.Search(searchText, searchOptions);
        var firstPage = searchResult.PagedResult.AsPages().FirstOrDefault();

        TestContext.WriteLine("Search text: " + searchText);
        if (testCase.ExpectedToFound)
        {
            searchResult.TotalCount.Should().Be(1);
            firstPage.Should().NotBeNull();
            var values = firstPage!.Values.ToList();
            values.Count.Should().Be(1);

            var document = values.First();
            document.Document.Description.Should().Be(searchValue,
                $"Search text: \"{searchText}\" should match the found document description");
        }
        else
        {
            searchResult.TotalCount.Should().Be(0, $"We should not find anything by this search text: {searchText}");
            firstPage.Should().BeNull();
        }
    }

    private static TestDocument ADocumentWith(string id, char value) => new TestDocumentBuilder()
        .WithDescription($"FirstPart {value} SecondPart").WithId(id).Build();

    private static void WaitIndexToBeUpdated() => Thread.Sleep(TimeSpan.FromSeconds(2));
}
