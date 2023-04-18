using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch;

public interface IAzureSearchBuilder
{
    public IServiceCollection Services { get; }
}