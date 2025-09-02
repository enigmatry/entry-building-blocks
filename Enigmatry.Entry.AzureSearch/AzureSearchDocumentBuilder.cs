using Enigmatry.Entry.AzureSearch.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch;

public class AzureSearchDocumentBuilder<T> : IAzureSearchBuilder
{
    public AzureSearchDocumentBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IAzureSearchBuilder AddSearchIndexFactory<TFactory>() where TFactory : class, ISearchIndexBuilder<T>
    {
        Services.AddScoped<ISearchIndexBuilder<T>, TFactory>();
        return this;
    }

    public IServiceCollection Services { get; }
}
