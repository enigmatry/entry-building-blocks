using Fluid;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public class FluidTemplateProvider(ILogger<FluidTemplateProvider> logger, FluidParser parser) : IFluidTemplateProvider
{
    public IFluidTemplate GetTemplate(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        try
        {
            return parser.Parse(text);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Template parsing failed");
            throw;
        }
    }
}
