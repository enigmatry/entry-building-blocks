using Enigmatry.Entry.Core.Helpers;

namespace Enigmatry.Entry.AzureSearch;

public record SearchText
{
    private SearchText(string originalValue, string value)
    {
        OriginalValue = originalValue.ToEmptyIfNull();
        Value = value.ToEmptyIfNull();
    }

    public static SearchText AsNotEscaped(string searchText) => new(searchText, searchText);

    public static SearchText AsEscaped(string searchText)
    {
        var result = searchText.ToEmptyIfNull().EscapeSpecialSymbols();
        return new SearchText(searchText, result);
    }

    public static SearchText AsPhraseSearch(string searchText)
    {
        var result = searchText.ToEmptyIfNull().EscapeSpecialSymbols().AsPhraseSearch();
        return new SearchText(searchText, result);
    }

    public static SearchText AsFullSearch(string searchText)
    {
        var result = searchText.ToEmptyIfNull().EscapeSpecialSymbols().AsFullSearch();
        return new SearchText(searchText, result);
    }

    public string OriginalValue { get; }
    public string Value { get; }
}
