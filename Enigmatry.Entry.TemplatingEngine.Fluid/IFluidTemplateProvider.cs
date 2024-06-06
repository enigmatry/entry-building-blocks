using Fluid;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public interface IFluidTemplateProvider
{
    IFluidTemplate GetTemplate(string text);
}
