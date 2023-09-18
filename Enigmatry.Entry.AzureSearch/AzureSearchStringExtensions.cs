using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.Entry.AzureSearch;

internal static class AzureSearchStringExtensions
{
    // Backslash must be the first symbol in the array, to avoid double escaping.
    internal static readonly string[] AzureSearchSpecialCharacters =
    {
        "\\", "+", "-", "&", "|", "!", "(", ")", "{", "}", "[", "]", "^", "\"", "~", "*", "?", ":", "/"
    };

    internal static string EscapeSpecialSymbols(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        // Check if the value contains any of the illegal symbols, and if it does escape them.
        foreach (var symbol in AzureSearchSpecialCharacters)
        {
            value = value.Replace(symbol, "\\" + symbol, StringComparison.InvariantCulture);
        }

        return value;
    }

    internal static string AsFullSearch(this string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            return searchText;
        }

        var words = searchText.Split(' ');
        var result =
            $"{searchText.AsPhraseSearch()} OR {(words.Length > 1 ? words.SearchAll() : $"{searchText.PartialSearch()} OR {searchText.FuzzySearch()}")}";
        return result;
    }

    internal static string AsPhraseSearch(this string phrase)
    {
        if (string.IsNullOrEmpty(phrase))
        {
            return phrase;
        }

        return @$"""{phrase}""";
    }

    private static string SearchAll(this IEnumerable<string> words) =>
        $"({string.Join(' ', words.Select(word => $"+{word}"))})";

    private static string PartialSearch(this string word) => $"{word}*";

    //  for fuzzy search to work query type has to be full
    private static string FuzzySearch(this string word) => $"{word}~1";
}
