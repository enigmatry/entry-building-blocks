using Fluid;
using Fluid.Values;
using System.Globalization;

namespace Enigmatry.Entry.TemplatingEngine.Liquid.Filters;

public class FormatNullableNumberFluidFilter : IFluidFilter
{
    public string FilterName => "format_nullable_number";

    public ValueTask<FluidValue> Filter(FluidValue input, FilterArguments arguments, TemplateContext context)
    {
        if (input.IsNil())
        {
            return new StringValue(string.Empty);
        }
        if (arguments.At(0).IsNil())
        {
            return NilValue.Instance;
        }

        var format = arguments.At(0).ToStringValue();

        var culture = context.CultureInfo;

        if (!arguments.At(1).IsNil())
        {
            culture = CultureInfo.CreateSpecificCulture(arguments.At(1).ToStringValue());
        }

        return new StringValue(input.ToNumberValue().ToString(format, culture));
    }
}
