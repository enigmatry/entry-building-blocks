namespace Enigmatry.Entry.AzureSearch.Tests;

public record AddStatementTestCase(IEnumerable<string> Statements, string ExpectedFilter)
{
    public static AddStatementTestCase Create(IEnumerable<string> statements, string expectedFilter) => new(statements, expectedFilter);
}
