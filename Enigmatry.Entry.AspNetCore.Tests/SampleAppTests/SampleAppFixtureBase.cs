using Enigmatry.Entry.AspNetCore.Tests.SampleApp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
    "CA1001:Types that own disposable fields should be disposable",
    Justification = "Fields are disposed in Teardown method")]
public abstract class SampleAppFixtureBase
{
    private WebApplicationFactory<Program> _factory = null!;
    protected HttpClient Client { get; private set; } = null!;
    private readonly SampleAppSettings _settings;
    private WebApplicationFactory<Program> _app = null!;

    protected SampleAppFixtureBase(SampleAppSettings settings)
    {
        _settings = settings;
    }

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _app = _factory.WithWebHostBuilder(configuration =>
        {
            configuration.ConfigureServices(serviceCollection =>
            {
                var mvcBuilder = serviceCollection.AddControllers();
                Program.ConfigureMvc(mvcBuilder, _settings);
            });
        });

        Client = _app.CreateClient();
    }


    [TearDown]
    public void Teardown()
    {
        _app.Dispose();
        Client?.Dispose();
        _factory?.Dispose();
    }
}
