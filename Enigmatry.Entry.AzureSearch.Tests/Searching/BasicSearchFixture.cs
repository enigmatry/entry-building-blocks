using Azure.Search.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Documents;

namespace Enigmatry.Entry.AzureSearch.Tests.Searching;

[Category("unit")]
public class BasicSearchFixture : SearchServiceFixtureBase
{
    private TestDocument _document = null!;
    private TestDocument _document2 = null!;

    [SetUp]
    public new async Task Setup()
    {
        _document = ADocument();
        _document2 = AnotherDocument();

        await UpdateDocuments(new[] { _document, _document2 });
    }

    [Test]
    public async Task TestSearchById()
    {
        var searchResult = await Search(_document.Id);
        await Verify(searchResult, CreateVerifySettings());
    }

    [Test]
    public async Task TestSearchByName()
    {
        var searchResult = await Search(_document.Name);
        await Verify(searchResult, CreateVerifySettings());
    }

    [Test]
    public async Task TestSearchByIdWithOptions()
    {
        var searchResult = await Search(_document.Id, ASearchOptionsWithDefaultSizeAndSkip());
        await Verify(searchResult, CreateVerifySettings());
    }

    [Test]
    public async Task TestSearchByNameWithOptions()
    {
        var searchResult = await Search(_document.Name, ASearchOptionsWithDefaultSizeAndSkip());
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
        var searchResult = await Search("", searchOptions);
        await Verify(searchResult, CreateVerifySettings());
    }

    [Test]
    public async Task TestSearchByDescription()
    {
        var searchResult = await Search(_document.Description);
        await Verify(searchResult, CreateVerifySettings());
    }

    private static TestDocument ADocument() => new TestDocumentBuilder().Build();

    private static TestDocument AnotherDocument() =>
        new TestDocumentBuilder().WithId("23432432").WithName("Harry Potter").WithDescription("Hogwarts")
            .Build();

    private static SearchOptions ASearchOptionsWithDefaultSizeAndSkip() =>
        new() { Size = 10, Skip = 0, IncludeTotalCount = true };
}
