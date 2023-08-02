using System.Net;
using FluentAssertions;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests.Authorization;

[Category("integration")]
internal class UnauthenticatedUserFixture : SampleAppFixtureBase
{
    public UnauthenticatedUserFixture()
    {
        AsUnauthenticatedUser();
    }

    [Test]
    public async Task TestEndpointWithAuthorizeIsNotAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/userNoPermissionIsNotAllowed");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task TestEndpointWithoutAuthorizeIsNotAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/noAuthorizeAttribute");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task TestEndpointWithAllowAnonymousIsAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/allowAnonymousAttribute");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task TestSwaggerUiIsAllowed()
    {
        var response = await Client.GetAsync("index.html");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task TestHealthCheckIsAllowed()
    {
        var response = await Client.GetAsync("healthcheck");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
