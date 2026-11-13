using System.Net;
using Shouldly;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests.Authorization;

[Category("integration")]
internal class UnauthenticatedAccessFixture : SampleAppFixtureBase
{
    public UnauthenticatedAccessFixture()
    {
        DisableUserAuthentication();
    }

    [Test]
    public async Task EndpointWithAuthorizeAttributeIsNotAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/userNoPermissionIsNotAllowed");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task EndpointWithoutAuthorizeAttributeIsNotAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/noAuthorizeAttribute");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task EndpointWithAllowAnonymousAttributeIsAllowed()
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
