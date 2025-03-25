using System.Net;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

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
        response!.Length.ShouldBe(5);
    }

    [Test]
    public async Task TestGetError()
    {
        var response = await Client.GetAsync("WeatherForecast/ThrowsError");

        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.ShouldBeNull();
    }

    [Test]
    public async Task TestGetNotFoundError()
    {
        var response = await Client.GetAsync("WeatherForecast/throwsEntityNotFoundException");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.ShouldBeNull();
    }

    [Test]
    public async Task TestGetNotFound()
    {
        var response = await Client.GetAsync("WeatherForecast/NotFound");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.ShouldBeNull();
    }

    [Test]
    public async Task TestGetValidationError()
    {
        var response = await Client.GetAsync("WeatherForecast/ProblemDetails");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.ShouldBe("application/problem+json");

        var json = await response.Content.ReadAsStringAsync();
        var problemDetails = DeserializeJson<ProblemDetails>(json)!;
        problemDetails.Extensions.Clear();

        await Verify(problemDetails);
    }
}
