namespace Enigmatry.Entry.AzureSearch.Tests;

public static class AzureSearchTestCases
{
    public static readonly char[] AzureSearchSpecialCharacters =
    {
        '+', '-', '&', '|', '!', '(', ')', '{', '}', '[', ']', '^', '"', '~', '*', '?', ':', '\\', '/'
    };

    public static IEnumerable<AzureSearchTestCase> AzureSearchSpecialCharactersTestCases => AzureSearchSpecialCharacters
        .Select(c => new AzureSearchTestCase(c, true));

    public record AzureSearchTestCase(char Value, bool ExpectedToFound);
}
