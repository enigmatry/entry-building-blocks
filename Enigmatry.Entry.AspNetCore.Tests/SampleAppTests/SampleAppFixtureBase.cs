using Enigmatry.Entry.AspNetCore.Tests.SampleApp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Enigmatry.Entry.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;

[SuppressMessage("Design",
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
            configuration.ConfigureServices(services =>
            {
                var mvcBuilder = services.AddControllers();
                Program.ConfigureMvc(mvcBuilder, _settings);
                if (_settings.AuthenticationEnabled)
                {
                    services.AddAuthentication(TestUserAuthenticationHandler.AuthenticationScheme)
                        .AddScheme<AuthenticationSchemeOptions, TestUserAuthenticationHandler>(
                            TestUserAuthenticationHandler.AuthenticationScheme, _ => { });

                    services.AppAddAuthorization<PermissionId>();
                    services.AddScoped<IAuthorizationProvider<PermissionId>, SampleAuthorizationProvider>();
                }
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
