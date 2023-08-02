using System.Net;
using FluentAssertions;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests.Authorization;

[Category("integration")]
internal class AuthenticatedUserFixture : SampleAppFixtureBase
{
    [Test]
    public async Task TestUserWithPermissionIsAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/userWithPermissionIsAllowed");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task TestUserNoPermissionIsNotAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/userNoPermissionIsNotAllowed");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task TestEndpointWithoutAuthorizeIsAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/noAuthorizeAttribute");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
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
