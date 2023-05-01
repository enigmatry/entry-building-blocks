using System;
using System.Linq;

namespace Enigmatry.Entry.AzureSearch;

internal static class AzureSearchStringExtensions
{
    public static string ProcessKeywordForAzureSearch(this string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            return String.Empty;
        }

        value = value.EscapeSpecialSymbols();

        var words = value.Split(' ');

        return
            $"{value.AsPhraseSearch()} OR {(words.Length > 1 ? SearchAll(words) : $"{PartialSearch(value)} OR {FuzzySearch(value)}")}";
    }

    public static string EscapeSpecialSymbols(this string value)
    {
        // Backslash must be the first symbol in the array, to avoid double escaping.
        var symbols = new[]
        {
            "\\", "+", "-", "&&", "||", "!", "(", ")", "{", "}", "[", "]", "^", "\"", "~", "*", "?", ":", "/"
        };

        // Check if the value contains any of the illegal symbols, and if it does escape them.
        foreach (var symbol in symbols)
        {
            value = value.Replace(symbol, "\\" + symbol, StringComparison.InvariantCulture);
        }

        return value;
    }

    public static string AsPhraseSearch(this string phrase) => @$"""{phrase}""";

    public static string SearchAll(this string[] words) => $"({String.Join(' ', words.Select(word => $"+{word}"))})";

    public static string PartialSearch(this string word) => $"{word}*";

    //  for fuzzy search to work query type has to be full
    public static string FuzzySearch(this string word) => $"{word}~1";
}
