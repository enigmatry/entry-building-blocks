using Enigmatry.Entry.AzureSearch.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch;

internal class AzureSearchBuilder : IAzureSearchBuilder
{
    public AzureSearchBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}
