using Enigmatry.Entry.AspNetCore.Tests.SampleApp;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp.Controllers;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;

[Category("integration")]
public abstract class WeatherForecastControllerFixtureBase : SampleAppFixtureBase
{
    protected Task GetAsync() => GetAsync<WeatherForecast[]>(Client, "WeatherForecast");

    protected async Task TestGetError()
    {
        var response = await Client.GetAsync("WeatherForecast/ThrowsError");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.Should().BeNull();
    }

    protected async Task TestGetNotFoundError()
    {
        var response = await Client.GetAsync("WeatherForecast/throwsEntityNotFoundException");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.Should().BeNull();
    }

    protected async Task TestGetNotFound()
    {
        var response = await Client.GetAsync("WeatherForecast/NotFound");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.Should().BeNull();
    }

    protected Task<HttpResponseMessage> TestGetValidationErrorsAsProblemDetails() =>
        Client.GetAsync("WeatherForecast/ProblemDetails");

    protected Task<HttpResponseMessage> TestPostInvalidRequest() =>
        PostWithoutResponseCheckAsync(Client, "WeatherForecast", AnInvalidRequest());

    protected Task<HttpResponseMessage> TestPostIncompatibleRequestAsync() =>
        // this request will not go to the controller because the request is incompatible
        // it will be captures early by the MVC binding phase and ModelState will be invalid
        // we expect a 400 with a problem details response
        PostWithoutResponseCheckAsync(Client, "WeatherForecast", AnIncompatibleRequest());

    private static UpdateWeatherForecast.Request AnInvalidRequest() => new();

    private static UpdateWeatherForecastIncompatibleRequest AnIncompatibleRequest() => new()
    {
        IntProperty = new Guid("A1BC4FFB-005A-4752-A481-21E84257CC72"),
        GuidProperty = 42,
        StringProperty = "Hello World"
    };

    protected async Task<(string responseJson, ProblemDetails problemDetails)> VerifyProblemDetailsAsResponse(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var json = await response.Content.ReadAsStringAsync();
        var problemDetails = DeserializeJson<ProblemDetails>(json)!;
        return (json, problemDetails);
    }

    protected abstract Task<T?> GetAsync<T>(HttpClient client, string uri);

    protected abstract Task<HttpResponseMessage> PostWithoutResponseCheckAsync<T>(HttpClient client, string uri,
        T content);

    protected abstract T? DeserializeJson<T>(string content);

    private class UpdateWeatherForecastIncompatibleRequest
    {
        // incompatible request has different types of properties to the original request 
        public int? GuidProperty { get; set; }
        public string? StringProperty { get; set; }
        public Guid? IntProperty { get; set; }
    }
}
