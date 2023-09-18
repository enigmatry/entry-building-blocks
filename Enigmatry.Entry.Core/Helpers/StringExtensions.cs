using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Enigmatry.Entry.Core.Helpers;

public static class StringExtensions
{
    public static bool HasNoContent(this string value) => !value.HasContent();
    public static bool HasContent(this string value) => !string.IsNullOrEmpty(value);

    public static bool Contains(this string source, string value, StringComparison comparisonType) =>
        source.Contains(value, comparisonType);

    public static bool IsNullOrWhiteSpace(this string source) => string.IsNullOrWhiteSpace(source);

    public static string ToEmptyIfNull(this string? value) => value ?? string.Empty;

    public static string Capitalize(this string value)
    {
        if (value.HasNoContent())
        {
            return value;
        }

        var firstUpperCharacter = char.ToUpper(value[0], CultureInfo.CurrentCulture);
        return value.Length != 1 ? $"{firstUpperCharacter}{value.Substring(1)}" : firstUpperCharacter.ToString();
    }

    public static string ToCamelCase(this string s)
    {
        if (s.HasNoContent() || !char.IsUpper(s[0]))
        {
            return s;
        }

        var chars = s.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i]))
            {
                break;
            }

            var hasNext = i + 1 < chars.Length;
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                break;
            }

            chars[i] = char.ToLower(chars[i], CultureInfo.CurrentCulture);
        }

        return new string(chars);
    }

    public static string JoinStringWithOnlyValuesWithContent(this IEnumerable<string> values, string separator) =>
        string.Join(separator, values.Where(value => value.HasContent()));
}
