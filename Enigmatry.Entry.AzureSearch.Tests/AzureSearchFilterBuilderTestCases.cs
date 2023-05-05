namespace Enigmatry.Entry.AzureSearch.Tests;

public static class AzureSearchFilterBuilderTestCases
{
    public static IEnumerable<AddStatementTestCase> FilterStatements =>
        new List<AddStatementTestCase>
        {
            AddStatementTestCase.Create(new List<string>(), ""),
            AddStatementTestCase.Create(new List<string> { "" }, ""),
            AddStatementTestCase.Create(new List<string> { "one" }, "(one)"),
            AddStatementTestCase.Create(new List<string> { "one", "two" }, "(one) and (two)")
        };
}
