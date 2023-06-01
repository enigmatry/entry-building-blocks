using System.Net;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp;
using Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;
using FluentAssertions;

namespace Enigmatry.Entry.AspNetCore.Tests;

[Category("integration")]
internal class AuthorizationFixture : SampleAppFixtureBase
{
    public AuthorizationFixture() : base(new SampleAppSettings() { UseNewtonsoftJson = false, AuthenticationEnabled = true })
    {
    }

    [Test]
    public async Task TestUserInRoleIsAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/userInRoleIsAllowed");
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task TestUserNotInRoleIsNotAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/userNotInRoleIsNotAllowed");
        response!.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task TestUserWithPermissionIsAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/userWithPermissionIsAllowed");
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task TestUserWithPermissionIsNotAllowed()
    {
        var response = await Client.GetAsync("WeatherForecast/userNoPermissionIsNotAllowed");
        response!.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task TestFallbackPolicy()
    {
        var response = await Client.GetAsync("WeatherForecast/unprotected");
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
