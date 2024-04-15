using Fluid;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public class FluidTemplateProvider : IFluidTemplateProvider
{
    private readonly ILogger<FluidTemplateProvider> _logger;
    private readonly FluidParser _parser;

    public FluidTemplateProvider(ILogger<FluidTemplateProvider> logger, FluidParser parser)
    {
        _logger = logger;
        _parser = parser;
    }

    public IFluidTemplate GetTemplate(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        try
        {
            return _parser.Parse(text);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Template parsing failed");
            throw;
        }
    }
}
