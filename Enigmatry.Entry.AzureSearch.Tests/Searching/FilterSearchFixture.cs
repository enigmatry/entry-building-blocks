using Azure.Search.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Documents;

namespace Enigmatry.Entry.AzureSearch.Tests.Searching;

[Category("integration")]
[Explicit("Flaky: TODO BP-815: Remove Explicit attribute")]
public class FilterSearchFixture : SearchServiceFixtureBase
{
    [SetUp]
    public new async Task Setup()
    {
        var documents = ATestDocuments();
        await UpdateDocuments(documents);
    }

    [Test]
    public async Task TestSearchWithEmptyFilter()
    {
        var filterBuilder = new AzureSearchFilterBuilder();

        var searchResult = await Search(filterBuilder);

        await Verify(searchResult);
    }

    [TestCase(null)]
    [TestCase("")]
    public async Task TestSearchWithEmptyStatement(string? statement)
    {
        var filterBuilder = new AzureSearchFilterBuilder();
        filterBuilder.AddStatement(statement);

        var searchResult = await Search(filterBuilder);

        await Verify(searchResult);
    }

    [Test]
    public async Task TestSearchWithAnd()
    {
        var filterBuilder = new AzureSearchFilterBuilder();
        filterBuilder.AddStatement($"{nameof(TestSearchRequest.Name)} eq 'name1'");
        filterBuilder.AddStatement("not search.ismatch('another')");

        var searchResult = await Search(filterBuilder);

        await Verify(searchResult);
    }

    [Test]
    public async Task TestSearchWithOr()
    {
        var filterBuilder = new AzureSearchFilterBuilder();
        var searchRequests = new List<TestSearchRequest>() { new("name1"), new("another") };
        filterBuilder.AddStatements(searchRequests, request => $"{nameof(TestDocument.Name)} eq '{request.Name}'");

        var searchResult = await Search(filterBuilder);

        await Verify(searchResult);
    }

    private async Task<SearchResponse<TestDocument>> Search(AzureSearchFilterBuilder filterBuilder)
    {
        var options = ASearchOptionsWithFilter(filterBuilder.Build());
        var search = SearchText.AsNotEscaped("*");
        var searchResult = await Search(search, options);
        return searchResult;
    }

    private static async Task Verify(SearchResponse<TestDocument> searchResult)
    {
        var settings = CreateVerifySettings();
        await Verifier.Verify(searchResult, settings);
    }

    private static SearchOptions ASearchOptionsWithFilter(string filter)
    {
        TestContext.Out.WriteLine(filter);
        var options = new SearchOptions
        {
            Skip = 0,
            Size = 10,
            IncludeTotalCount = true,
            Filter = filter,
            OrderBy = { "Id" }
        };
        return options;
    }

    private static IEnumerable<TestDocument> ATestDocuments()
    {
        yield return ADocument("1", "name1", "some description");
        yield return ADocument("2", "name1", "some description");
        yield return ADocument("3", "name1", "some description");
        yield return ADocument("4", "name1", "some description");
        yield return ADocument("5", "another", "some description");
        yield return ADocument("6", "another", "some description");
    }

    private static TestDocument ADocument(string id, string name, string description) => new TestDocumentBuilder()
        .WithId(id).WithName(name).WithDescription(description).Build();
}
