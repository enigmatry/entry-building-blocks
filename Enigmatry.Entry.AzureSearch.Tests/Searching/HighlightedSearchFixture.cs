using Azure.Search.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Documents;

namespace Enigmatry.Entry.AzureSearch.Tests.Searching;

[Category("integration")]
[Explicit("Flaky: TODO BP-815: Remove Explicit attribute")]
public class HighlightedSearchFixture : SearchServiceFixtureBase
{
    [SetUp]
    public new async Task Setup()
    {
        var documents = ATestDocuments();
        await UpdateDocuments(documents);
    }

    [TestCase("*")]
    [TestCase("")]
    [TestCase(null)]
    [TestCase("name1")]
    [TestCase("lorem")]
    public async Task TestSearchWithHighlights(string? searchText)
    {
        var options = ASearchOptionsWithHighlighting();

        var search = SearchText.AsNotEscaped(searchText);

        var searchResult = await Search(search, options);
        var settings = CreateVerifySettings();
        await Verify(searchResult, settings);
    }

    private static SearchOptions ASearchOptionsWithHighlighting()
    {
        var options = new SearchOptions { Skip = 0, Size = 10, IncludeTotalCount = true, OrderBy = { "Id" } };
        // https://learn.microsoft.com/en-us/azure/search/search-pagination-page-layout#hit-highlighting
        options.HighlightFields.Add($"{nameof(TestDocument.Name)}, {nameof(TestDocument.Description)}");
        options.HighlightPreTag = "<b>";
        options.HighlightPostTag = "</b>";
        return options;
    }

    private static IEnumerable<TestDocument> ATestDocuments()
    {
        yield return ADocument("1", "name1", "description");
        yield return ADocument("2", "some other name", "lorem ipsum");
    }

    private static TestDocument ADocument(string id, string name, string description) => new TestDocumentBuilder()
        .WithId(id).WithName(name).WithDescription(description).Build();
}
