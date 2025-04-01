using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Enigmatry.Entry.Core.Helpers;

public static class StringExtensions
{
    public static bool HasNoContent(this string? value) => !value.HasContent();
    public static bool HasContent(this string? value) => !string.IsNullOrEmpty(value);

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

    public static string JoinNonEmptyStrings(this IEnumerable<string> values, string separator) =>
        string.Join(separator, values.Where(value => value.HasContent()));

    public static int ClosestPrimeToLength(this string value)
    {
        int length = value?.Length ?? 0;
        if (length < 2) return 2;

        bool IsPrime(int num)
        {
            if (num < 2) return false;
            for (int i = 2; i <= Math.Sqrt(num); i++)
                if (num % i == 0) return false;
            return true;
        }

        int lower = length, upper = length;
        while (true)
        {
            if (IsPrime(lower)) return lower;
            if (IsPrime(upper)) return upper;
            lower--;
            upper++;
        }
    }

    public static string ToKebabCase(this string value)
    {
        if (value.HasNoContent())
        {
            return value;
        }

        var result = new List<char>();
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0 && (char.IsLower(value[i - 1]) || (i + 1 < value.Length && char.IsLower(value[i + 1]))))
                {
                    result.Add('-');
                }
                result.Add(char.ToLower(c, CultureInfo.CurrentCulture));
            }
            else
            {
                result.Add(c);
            }
        }

        return new string(result.ToArray());
    }
}
