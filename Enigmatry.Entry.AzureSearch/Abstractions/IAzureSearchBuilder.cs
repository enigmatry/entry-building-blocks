using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch.Abstractions;

public interface IAzureSearchBuilder
{
    public IServiceCollection Services { get; }
}
