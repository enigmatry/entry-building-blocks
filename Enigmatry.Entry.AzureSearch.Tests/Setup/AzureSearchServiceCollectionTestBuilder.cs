using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AzureSearch.Tests.Setup;

public class AzureSearchServiceCollectionTestBuilder
{
    private const string AzureSearchApiKey = "AzureSearch:ApiKey";
    private const string AzureSearchSearchServiceEndpointKey = "AzureSearch:SearchServiceEndPoint";

    private readonly ServiceCollection _services = new();
    public ServiceProvider Build()
    {
        _services.AddLogging(configure => configure.AddConsole());

        var configuration = CreateConfiguration();
        _services.AddScoped<IConfiguration>(_ => configuration);

        _services.AddAzureSearch(configure =>
            {
                configure.ApiKey = configuration[AzureSearchApiKey];
                configure.SearchServiceEndPoint = new Uri(configuration[AzureSearchSearchServiceEndpointKey]);
            })
            .AddDocument<TestDocument>()
            .AddSearchIndexFactory<TestDocumentSearchIndexFactory>()
            .AddDocument<AnotherTestDocument>();

        return _services.BuildServiceProvider();
    }

    private static IConfigurationRoot CreateConfiguration()
    {
        var configurationSource = new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string>
            {
                { AzureSearchApiKey,  "USER_SECRET"},
                { AzureSearchSearchServiceEndpointKey, "USER_SECRET" }
            }
        };

        var configurationBuilder = new ConfigurationBuilder()
            .Add(configurationSource)
            .AddUserSecrets<AzureSearchServiceCollectionTestBuilder>()
            .AddEnvironmentVariables();

        return configurationBuilder.Build();
    }
}
