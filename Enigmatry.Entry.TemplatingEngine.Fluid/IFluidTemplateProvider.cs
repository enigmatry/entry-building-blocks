using Fluid;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public interface IFluidTemplateProvider
{
    public IFluidTemplate GetTemplate(string text);
}
