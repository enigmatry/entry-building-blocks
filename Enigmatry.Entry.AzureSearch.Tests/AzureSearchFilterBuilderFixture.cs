using System.Globalization;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using FluentAssertions;

namespace Enigmatry.Entry.AzureSearch.Tests;

[Category("unit")]
public class AzureSearchFilterBuilderFixture
{
    [TestCaseSource(typeof(AzureSearchFilterBuilderTestCases),
        nameof(AzureSearchFilterBuilderTestCases.FilterStatements))]
    public void TestAddStatement(AddStatementTestCase testCase)
    {
        var filterBuilder = new AzureSearchFilterBuilder();

        foreach (var statement in testCase.Statements)
        {
            filterBuilder.AddStatement(statement);
        }

        var filter = filterBuilder.Build();
        filter.Should().Be(testCase.ExpectedFilter);
    }

    [Test]
    public void TestAddStatements()
    {
        var filterBuilder = new AzureSearchFilterBuilder();

        var documents = new List<TestDocument> { ADocument(1, "FirstDocument"), ADocument(2, "SecondDocument") };

        filterBuilder.AddStatements(documents, document => $"{nameof(TestDocument.Name)} neq '{document.Id}'");
        filterBuilder.AddStatements(documents, document => $"{nameof(TestDocument.Id)} eq '{document.Id}'");

        var filter = filterBuilder.Build();
        filter.Should().Be("(Name neq '1' or Name neq '2') and (Id eq '1' or Id eq '2')");
    }

    private static TestDocument ADocument(int id, string name) => new TestDocumentBuilder()
        .WithId(id.ToString(CultureInfo.InvariantCulture)).WithName(name).Build();
}
