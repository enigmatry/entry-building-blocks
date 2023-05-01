using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.Entry.AzureSearch;

internal static class AzureSearchStringExtensions
{
    // Backslash must be the first symbol in the array, to avoid double escaping.
    public static readonly string[] AzureSearchSpecialCharacters =
    {
        "\\", "+", "-", "&", "|", "!", "(", ")", "{", "}", "[", "]", "^", "\"", "~", "*", "?", ":", "/"
    };

    public static string EscapeSpecialSymbols(this string value)
    {
        // Check if the value contains any of the illegal symbols, and if it does escape them.
        foreach (var symbol in AzureSearchSpecialCharacters)
        {
            value = value.Replace(symbol, "\\" + symbol, StringComparison.InvariantCulture);
        }

        return value;
    }

    public static string AsPhraseSearch(this string phrase) => @$"""{phrase}""";

    public static string SearchAll(this IEnumerable<string> words) => $"({String.Join(' ', words.Select(word => $"+{word}"))})";

    public static string PartialSearch(this string word) => $"{word}*";

    //  for fuzzy search to work query type has to be full
    public static string FuzzySearch(this string word) => $"{word}~1";
}
