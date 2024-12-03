using System.Globalization;

namespace Enigmatry.Entry.Csv;

public class CsvHelperOptions
{
    public CultureInfo Culture { get; private set; } = CultureInfo.CurrentCulture;
    public Func<string, string> HeaderNameReplacer { get; private set; } = name => name;

    public CsvHelperOptions WithCulture(CultureInfo culture)
    {
        Culture = culture;
        return this;
    }

    public CsvHelperOptions WithHeaderNameReplacer(Func<string, string> headerNameReplacer)
    {
        HeaderNameReplacer = headerNameReplacer;
        return this;
    }
}
