using Enigmatry.Entry.AspNetCore.Tests.SampleApp;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;

[Category("integration")]
public abstract class WeatherForecastControllerFixtureBase : SampleAppFixtureBase
{
    protected abstract Task<T?> GetAsync<T>(HttpClient client, string uri);

    protected abstract T? DeserializeJson<T>(string content);

    [Test]
    public async Task TestGet()
    {
        var response = await GetAsync<WeatherForecast[]>(Client, "WeatherForecast");
        response!.Length.Should().Be(5);
    }

    [Test]
    public async Task TestGetError()
    {
        var response = await Client.GetAsync("WeatherForecast/ThrowsError");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.Should().BeNull();
    }

    [Test]
    public async Task TestGetNotFound()
    {
        var response = await Client.GetAsync("WeatherForecast/NotFound");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.Should().BeNull();
    }

    [Test]
    public async Task TestGetValidationError()
    {
        var response = await Client.GetAsync("WeatherForecast/ProblemDetails");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var json = await response.Content.ReadAsStringAsync();
        var problemDetails = DeserializeJson<ProblemDetails>(json)!;
        problemDetails.Extensions.Clear();

        await Verify(problemDetails);
    }
}
