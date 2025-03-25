using System.Net;
using Shouldly;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests.Authorization;

[Category("integration")]
internal class AuthorizationFixture : SampleAppFixtureBase
{
    [Test]
    public async Task UserWithPermissionIsAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/userWithPermissionIsAllowed");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task UserNoPermissionIsNotAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/userNoPermissionIsNotAllowed");
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task EndpointWithoutAuthorizeIsAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/noAuthorizeAttribute");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task EndpointWithAllowAnonymousIsAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/allowAnonymousAttribute");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task SwaggerUiIsAllowed()
    {
        var response = await Client.GetAsync("index.html");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task HealthCheckIsAllowed()
    {
        var response = await Client.GetAsync("healthcheck");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
