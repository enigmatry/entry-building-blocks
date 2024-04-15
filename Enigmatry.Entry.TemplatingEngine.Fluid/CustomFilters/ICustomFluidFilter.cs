using Fluid;
using Fluid.Values;

namespace Enigmatry.Entry.TemplatingEngine.Liquid.CustomFilters;

public interface ICustomFluidFilter
{
    public string FilterName { get; }
    public ValueTask<FluidValue> Filter(FluidValue input, FilterArguments arguments, TemplateContext context);
}
