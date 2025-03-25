using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace Enigmatry.Entry.Csv;

[PublicAPI]
public class CsvHelperOptions
{
    public Encoding Encoding { get; set; } = Encoding.UTF8;
    public CultureInfo Culture { get; private set; } = CultureInfo.CurrentCulture;
    public Func<string, string> HeaderNameReplacer { get; private set; } = name => name;

    public CsvHelperOptions WithEncoding(Encoding encoding)
    {
        Encoding = encoding;
        return this;
    }

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
