using System.Globalization;

namespace Enigmatry.Entry.Csv;

public class CsvHelperOptions
{
    public CultureInfo Culture { get; private set; } = CultureInfo.CurrentCulture;
    public Func<string, string> ColumnNameReplacer { get; private set; } = name => name;

    public CsvHelperOptions WithCulture(CultureInfo culture)
    {
        Culture = culture;
        return this;
    }

    public CsvHelperOptions WithColumnNameReplacer(Func<string, string> nameReplacer)
    {
        ColumnNameReplacer = nameReplacer;
        return this;
    }
}
