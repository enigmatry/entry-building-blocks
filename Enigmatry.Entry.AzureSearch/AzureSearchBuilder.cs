using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch;

public class AzureSearchBuilder : IAzureSearchBuilder
{
    public AzureSearchBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}