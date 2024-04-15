using Fluid;
using Fluid.Values;
using JetBrains.Annotations;
using System.Globalization;

namespace Enigmatry.Entry.TemplatingEngine.Liquid.CustomFilters;

[UsedImplicitly]
public class ToCurrencyCustomFluidFilter : ICustomFluidFilter
{
    public string FilterName => "to_currency";

    public ValueTask<FluidValue> Filter(FluidValue input, FilterArguments arguments, TemplateContext context)
    {
        if (input.IsNil())
        {
            return StringValue.Create(string.Empty);
        }

        var numberFormatInfo = new NumberFormatInfo
        {
            NumberDecimalDigits = ResolvePrecision(arguments),
            NumberGroupSeparator = ResolveThousandsDelimiter(arguments),
            NumberDecimalSeparator = ResolveDecimalSeparator(arguments)
        };

        var currency = ResolveCurrency(arguments);
        return StringValue.Create(currency + input.ToNumberValue().ToString("n", numberFormatInfo));
    }

    private static class ArgumentsPositionIndex
    {
        public static int Currency => 0;
        public static int Precision => 1;
        public static int ThousandsSeparator => 2;
        public static int DecimalSeparator => 3;
    }

    private static class DefaultValues
    {
        public static string Currency => "";
        public static int Precision => 2;
        public static string DecimalSeparator => ".";
        public static string ThousandsDelimiter => ",";
    }

    private static int ResolvePrecision(FilterArguments arguments) =>
        arguments.At(ArgumentsPositionIndex.Precision).IsInteger()
            ? (int)arguments.At(ArgumentsPositionIndex.Precision).ToNumberValue()
            : DefaultValues.Precision;

    private static string ResolveThousandsDelimiter(FilterArguments arguments) =>
        !arguments.At(ArgumentsPositionIndex.ThousandsSeparator).IsNil()
            ? arguments.At(ArgumentsPositionIndex.ThousandsSeparator).ToStringValue()
            : DefaultValues.ThousandsDelimiter;

    private static string ResolveDecimalSeparator(FilterArguments arguments) =>
        !arguments.At(ArgumentsPositionIndex.DecimalSeparator).IsNil()
            ? arguments.At(ArgumentsPositionIndex.DecimalSeparator).ToStringValue()
            : DefaultValues.DecimalSeparator;

    private static string ResolveCurrency(FilterArguments arguments) =>
        !arguments.At(ArgumentsPositionIndex.Currency).IsNil()
            ? arguments.At(ArgumentsPositionIndex.Currency).ToStringValue()
            : DefaultValues.Currency;
}
