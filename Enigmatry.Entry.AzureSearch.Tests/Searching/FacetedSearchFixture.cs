using Azure.Search.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Documents;

namespace Enigmatry.Entry.AzureSearch.Tests.Searching;

public class FacetedSearchFixture : SearchServiceFixtureBase
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
    public async Task TestSearchWithFacets(string searchText)
    {
        var options = ASearchOptionsWithFaceting();

        var search = SearchText.AsNotEscaped(searchText);

        var searchResult = await Search(search, options);
        var settings = CreateVerifySettings();
        settings.DontScrubDateTimes();

        await Verify(searchResult, settings);
    }

    private static SearchOptions ASearchOptionsWithFaceting()
    {
        var options = new SearchOptions { Skip = 0, Size = 0, IncludeTotalCount = true };
        // facet syntax https://learn.microsoft.com/en-us/azure/search/search-faceted-navigation#facets-syntax
        // https://learn.microsoft.com/en-us/rest/api/searchservice/search-documents#query-parameters
        options.Facets.Add($"{nameof(TestDocument.Name)},count:100");
        options.Facets.Add($"{nameof(TestDocument.Rating)},interval:5");
        options.Facets.Add($"{nameof(TestDocument.CreatedOn)},values:2000-02-01T00:00:00Z");
        return options;
    }

    private static TestDocument ADocument(string id, string name, string description, int rating,
        DateTimeOffset createdOn) => new TestDocumentBuilder()
        .WithId(id).WithName(name).WithDescription(description).WithRating(rating).WithCreatedOn(createdOn).Build();

    private static IEnumerable<TestDocument> ATestDocuments()
    {
        yield return ADocument("1", "name1", "some description", 1, ADateInPast());
        yield return ADocument("2", "name1", "some description", 1, ADateInFuture());
        yield return ADocument("3", "name1", "some description", 5, ADateInPast());
        yield return ADocument("4", "name1", "some description", 10, ADateInPast());
        yield return ADocument("5", "another", "some description", 5, ADateInFuture());
        yield return ADocument("6", "another", "some description", 10, ADateInFuture());
    }

    private static DateTimeOffset ADateInFuture() => new(2050, 1, 1, 1, 1, 1, TimeSpan.Zero);

    private static DateTimeOffset ADateInPast() => new(1978, 1, 1, 1, 1, 1, TimeSpan.Zero);
}
