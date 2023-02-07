using System.Net;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;

[Category("integration")]
public abstract class WeatherForecastControllerFixtureBase : SampleAppFixtureBase
{
    private readonly bool _newtonsoftJsonUsed;

    protected WeatherForecastControllerFixtureBase(SampleAppSettings settings) : base(settings)
    {
        _newtonsoftJsonUsed = settings.UseNewtonsoftJson;
    }

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
        HttpResponseMessage response = await Client.GetAsync("WeatherForecast/NotFound");

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

        problemDetails.Status.Should().Be((int)HttpStatusCode.BadRequest);
        problemDetails.Title.Should().Be("One or more validation errors occurred.");
        problemDetails.Instance.Should().Be("/WeatherForecast/ProblemDetails");

        if (!_newtonsoftJsonUsed)
        {
            var errors = problemDetails.Extensions["errors"]!.ToString();
            errors.Should().Be("{\"AProperty\":[\"AFailedValidationMessage\"]}");
        }
        else
        {
            problemDetails.Extensions.Count.Should().Be(0, "NewtonsoftJson does not deserialize properly Extensions");
        }
    }
}
