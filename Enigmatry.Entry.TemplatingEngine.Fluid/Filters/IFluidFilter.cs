using Fluid;
using Fluid.Values;

namespace Enigmatry.Entry.TemplatingEngine.Liquid.Filters;

public interface IFluidFilter
{
    public string FilterName { get; }
    public ValueTask<FluidValue> Filter(FluidValue input, FilterArguments arguments, TemplateContext context);
}
