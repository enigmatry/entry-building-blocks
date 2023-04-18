using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch.Tests.Setup;

public class AzureSearchServiceCollectionTestBuilder
{
    private readonly ServiceCollection _serviceCollection = new();
    public ServiceProvider Build()
    {
        _serviceCollection.AddAzureSearch(configure =>
            {
                configure.ApiKey = "";
                configure.SearchServiceEndPoint = new Uri("https://enigmatry-dev.search.windows.net");
            })
            .AddDocument<TestDocument>()
            .AddSearchIndexFactory<TestDocumentSearchIndexFactory>()
            .AddDocument<AnotherTestDocument>();

        return _serviceCollection.BuildServiceProvider();
    }
}
