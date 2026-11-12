using System.Globalization;

namespace Enigmatry.Entry.TemplatingEngine.Liquid.ValueConverters;

// Fluid stores numbers as decimal; an exact double-to-decimal conversion exposes binary noise (1.23 renders as
// 1.2299999999999999822...), so convert at the 15/7 significant digits a double/float actually carries.
public class FloatingPointValueConverter : IFluidValueConverter
{
    public object? Convert(object? value) => value switch
    {
        double number => ToFluidNumber(number.ToString("G15", CultureInfo.InvariantCulture)),
        float number => ToFluidNumber(number.ToString("G7", CultureInfo.InvariantCulture)),
        _ => null
    };

    // NaN, infinities and magnitudes beyond decimal's range render as text instead of failing the render
    private static object ToFluidNumber(string number) =>
        decimal.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : number;
}
