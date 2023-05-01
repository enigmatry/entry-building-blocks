namespace Enigmatry.Entry.AzureSearch;

public record SearchText
{
    private SearchText(string value)
    {
        Value = value;
    }

    public static SearchText AsPhraseSearch(string value)
    {
        value = value.EscapeSpecialSymbols();
        var result = value.AsPhraseSearch();
        return new SearchText(result);
    }

    public static SearchText AsFullSearch(string value)
    {
        value = value.EscapeSpecialSymbols();
        var words = value.Split(' ');
        var result =
            $"{value.AsPhraseSearch()} OR {(words.Length > 1 ? words.SearchAll() : $"{value.PartialSearch()} OR {value.FuzzySearch()}")}";
        return new SearchText(result);
    }

    public string Value { get; init; }

    public void Deconstruct(out string value) => value = Value;
}
