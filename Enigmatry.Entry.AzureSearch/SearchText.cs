namespace Enigmatry.Entry.AzureSearch;

public record SearchText
{
    private SearchText(string valueRaw, string value)
    {
        ValueRaw = valueRaw;
        Value = value;
    }

    public static SearchText AsPhraseSearch(string value)
    {
        var result = value.EscapeSpecialSymbols();
        result = result.AsPhraseSearch();
        return new SearchText(value, result);
    }

    public static SearchText AsFullSearch(string value)
    {
        var result = value.EscapeSpecialSymbols();
        var words = result.Split(' ');
        result =
            $"{value.AsPhraseSearch()} OR {(words.Length > 1 ? words.SearchAll() : $"{value.PartialSearch()} OR {value.FuzzySearch()}")}";
        return new SearchText(value, result);
    }

    public string ValueRaw { get; init; }
    public string Value { get; init; }
}
