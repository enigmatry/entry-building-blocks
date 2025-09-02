using System.Globalization;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Enigmatry.Entry.AzureSearch.Extensions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using static Enigmatry.Entry.AzureSearch.Tests.Searching.AzureSearchTestCases.AzureSearchTestCase;

namespace Enigmatry.Entry.AzureSearch.Tests.Searching;

public static class AzureSearchTestCases
{
    public static IEnumerable<AzureSearchTestCase> AzureSearchSpecialCharactersTestCases
    {
        get
        {
            static string CreateDescription(string @char)
            {
                return $"FirstPart {@char} SecondPart";
            }

            var testDocuments = CreateDocumentsWithSpecialCharactersInDescription(CreateDescription).ToList();
            var pageSize = testDocuments.Count + 1; // ensure all documents are on first page to keep test simpler 
            var options = CreateSearchOptions(pageSize, SearchQueryType.Full);

            return AzureSearchStringExtensions
                .AzureSearchSpecialCharacters
                .Select(c => new AzureSearchTestCase($"{c} as special char",
                    SearchText.AsPhraseSearch(CreateDescription(c)),
                    options,
                    testDocuments,
                    TestCaseExpectation.Create(testDocuments.Count, 1,
                        testDocuments)));
        }
    }

    public static IEnumerable<AzureSearchTestCase> AzureSearchFullSearchTestCases
    {
        get
        {
            var documents = CreateDocumentsWithDescription(2, index => $"FirstPart{index} SecondPart{index}").ToList();
            var options = CreateSearchOptions(10, SearchQueryType.Full);

            yield return new AzureSearchTestCase("Full search test 1",
                SearchText.AsFullSearch("FirstPart1 SecondPart1"),
                options,
                documents,
                TestCaseExpectation.Create(1, 1,
                    documents.Where(t => t.Description == "FirstPart1 SecondPart1"),
                    "because we should find only this document"));

            yield return new AzureSearchTestCase("Full search test 2",
                SearchText.AsFullSearch("FirstPart2 SecondPart2"),
                options,
                documents,
                TestCaseExpectation.Create(1, 1,
                    documents.Where(t => t.Description == "FirstPart2 SecondPart2"),
                    "because we should find only this document"));

            yield return new AzureSearchTestCase("Full search test 3",
                SearchText.AsFullSearch("FirstPart3 SecondPart3"),
                options,
                documents,
                TestCaseExpectation.NoResults("because this document was not created"));

            yield return new AzureSearchTestCase("Full search test 4", SearchText.AsFullSearch("FirstPart1"),
                options,
                documents,
                TestCaseExpectation.Create(2, 1,
                    documents, "because all documents are found with Lucene search"));
        }
    }

    public static IEnumerable<AzureSearchTestCase> AzureSearchPhraseSearchTestCases
    {
        get
        {
            var documents = CreateDocumentsWithDescription(2, index => $"FirstPart{index} SecondPart{index}").ToList();
            var options = CreateSearchOptions(10, SearchQueryType.Full);

            yield return new AzureSearchTestCase("Phrase search test 1",
                SearchText.AsPhraseSearch("FirstPart1 SecondPart1"),
                options,
                documents,
                TestCaseExpectation.Create(1, 1,
                    documents.Where(t => t.Description == "FirstPart1 SecondPart1"),
                    "because we should find only this document"));

            yield return new AzureSearchTestCase("Phrase search test 2",
                SearchText.AsPhraseSearch("FirstPart2 SecondPart2"),
                options,
                documents,
                TestCaseExpectation.Create(1, 1,
                    documents.Where(t => t.Description == "FirstPart2 SecondPart2"),
                    "because we should find only this document"));

            yield return new AzureSearchTestCase("Phrase search test 3",
                SearchText.AsPhraseSearch("FirstPart3 SecondPart3"),
                options,
                documents,
                TestCaseExpectation.NoResults("because this document was not created"));

            yield return new AzureSearchTestCase("Phrase search test 4", SearchText.AsPhraseSearch("FirstPart1"),
                options,
                documents,
                TestCaseExpectation.Create(1, 1,
                    documents.Where(t => t.Description == "FirstPart1 SecondPart1")));
        }
    }

    private static SearchOptions CreateSearchOptions(int pageSize, SearchQueryType queryType) =>
        new()
        {
            IncludeTotalCount = true,
            Size = pageSize,
            Skip = 0,
            QueryType = queryType,
            SearchMode = SearchMode.All
        };

    private static IEnumerable<TestDocument> CreateDocumentsWithSpecialCharactersInDescription(
        Func<string, string> createDescription) =>
        AzureSearchStringExtensions.AzureSearchSpecialCharacters.Select((character, index) =>
            ADocumentWith(index, "some name", createDescription(character)));

    private static IEnumerable<TestDocument> CreateDocumentsWithDescription(int count,
        Func<int, string> createDescription) =>
        Enumerable.Range(1, count).Select(index =>
            ADocumentWith(index, "some name", createDescription(index)));

    private static TestDocument ADocumentWith(int id, string name, string description) => new TestDocumentBuilder()
        .WithId(id.ToString(CultureInfo.InvariantCulture)).WithName(name).WithDescription(description).Build();

    public record AzureSearchTestCase(string TestName, SearchText SearchText, SearchOptions Options,
        IEnumerable<TestDocument> Documents,
        TestCaseExpectation Expectation)
    {
        public override string ToString() =>
            $"{TestName} - SearchTextRaw: {SearchText.OriginalValue} - SearchText: {SearchText.Value}, Expectation: {Expectation}";

        public record TestCaseExpectation(int TotalCount, int PagesTotalCount,
            IEnumerable<TestDocument> ExpectedDocumentsOnFirstPage, string Reason)
        {
            public static TestCaseExpectation Create(int totalCount, int pagesTotalCount,
                IEnumerable<TestDocument> expectedDocumentsOnFirstPage, string reason = "") =>
                new(totalCount, pagesTotalCount, expectedDocumentsOnFirstPage, reason);

            public static TestCaseExpectation NoResults(string reason = "") =>
                new(0, 1, Enumerable.Empty<TestDocument>(), reason);

            public override string ToString() =>
                $"TotalCount: {TotalCount}, PagesTotalCount: {PagesTotalCount}";
        }
    }
}
