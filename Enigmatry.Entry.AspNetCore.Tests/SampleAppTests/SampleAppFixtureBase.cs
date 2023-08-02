using Enigmatry.Entry.AspNetCore.Tests.SampleApp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Enigmatry.Entry.AspNetCore.Tests.Infrastructure.TestImpersonation;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;

[SuppressMessage("Design",
    "CA1001:Types that own disposable fields should be disposable",
    Justification = "Fields are disposed in Teardown method")]
public abstract class SampleAppFixtureBase
{
    private WebApplicationFactory<Program> _factory = null!;
    private WebApplicationFactory<Program> _app = null!;

    protected HttpClient Client { get; private set; } = null!;
    private readonly SampleAppSettings _settings = SampleAppSettings.Default();

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _app = _factory.WithWebHostBuilder(configuration =>
        {
            configuration.ConfigureServices(services =>
            {
                var mvcBuilder = services.AddControllers();
                Program.ConfigureMvc(mvcBuilder, _settings);

                if (_settings.IsUserAuthenticated)
                {
                    services.AddTestUserAuthentication();
                }
                else
                {
                    services.AddNoResultAuthentication();
                }
            });
        });

        Client = _app.CreateClient();
    }

    protected void UseNewtonsoftJson() => _settings.UseNewtonsoftJson = true;

    protected void AsUnauthenticatedUser() => _settings.IsUserAuthenticated = false;


    [TearDown]
    public void Teardown()
    {
        _app.Dispose();
        Client.Dispose();
        _factory.Dispose();
    }
}
