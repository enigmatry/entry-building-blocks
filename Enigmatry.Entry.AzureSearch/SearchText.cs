namespace Enigmatry.Entry.AzureSearch;

public record SearchText
{
    private SearchText(string originalValue, string value)
    {
        OriginalValue = originalValue;
        Value = value;
    }

    public static SearchText AsEscaped(string searchText)
    {
        var result = searchText.EscapeSpecialSymbols();
        return new SearchText(searchText, result);
    }

    public static SearchText AsPhraseSearch(string searchText)
    {
        var result = searchText.EscapeSpecialSymbols().AsPhraseSearch();
        return new SearchText(searchText, result);
    }

    public static SearchText AsFullSearch(string searchText)
    {
        var result = searchText.EscapeSpecialSymbols().AsFullSearch();
        return new SearchText(searchText, result);
    }

    public string OriginalValue { get; }
    public string Value { get; }
}
