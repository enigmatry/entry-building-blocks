using Enigmatry.Entry.AspNetCore.Tests.SampleApp;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp.Controllers;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;

[Category("integration")]
public abstract class WeatherForecastControllerFixtureBase : SampleAppFixtureBase
{
    [SetUp]
    public void SetUp() => UseProjectRelativeDirectory("");

    [Test]
    public async Task TestGet()
    {
        var response = await GetAsync<WeatherForecast[]>(Client, "WeatherForecast");
        await Verify(response);
    }

    [Test]
    public async Task TestGetErrorResponse()
    {
        var response = await Client.GetAsync("WeatherForecast/ThrowsError");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.Should().BeNull();
    }

    [Test]
    public async Task TestGetNotFoundErrorResponse()
    {
        var response = await Client.GetAsync("WeatherForecast/throwsEntityNotFoundException");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.Should().BeNull();
    }

    [Test]
    public async Task TestGetNotFoundResponse()
    {
        var response = await Client.GetAsync("WeatherForecast/NotFound");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.Should().BeNull();
    }

    [Test]
    public async Task TestGetValidationErrorResponse()
    {
        var response = await Client.GetAsync("WeatherForecast/ProblemDetails");
        await VerifyProblemDetailsAsResponse(response);
    }

    [Test]
    public async Task TestPostInvalidRequest()
    {
        var response = await PostWithoutResponseCheckAsync(Client, "WeatherForecast", AnInvalidRequest());
        await VerifyProblemDetailsAsResponse(response);
    }

    [Test]
    public async Task TestPostIncompatibleRequestAsync()
    {
        // this request will not go to the controller because the request is incompatible
        // it will be captures early by the MVC binding phase and ModelState will be invalid
        // we expect a 400 with a problem details response
        var response = await PostWithoutResponseCheckAsync(Client, "WeatherForecast", AnIncompatibleRequest());
        await VerifyProblemDetailsAsResponse(response);
    }

    private static UpdateWeatherForecast.Request AnInvalidRequest() => new();

    private static UpdateWeatherForecastIncompatibleRequest AnIncompatibleRequest() => new()
    {
        IntProperty = new Guid("A1BC4FFB-005A-4752-A481-21E84257CC72"),
        GuidProperty = 42,
        StringProperty = "Hello World"
    };

    private async Task VerifyProblemDetailsAsResponse(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var json = await response.Content.ReadAsStringAsync();
        var problemDetails = DeserializeJson<ProblemDetails>(json)!;
        problemDetails.Should().NotBeNull();

        var modifiedJsonString = ScrubTraceIdPropertyFrom(json);

        await VerifyJson(modifiedJsonString);
    }

    private static string ScrubTraceIdPropertyFrom(string json)
    {
        // if we use ProblemDetails for Verify we don't get all the information since raw json contains
        // more data that is useful for verification 
        // unfortunately traceId is constantly changing, so we need to scrub it from the json
        var result = ScrubPropertyFromJson(json, "traceId");
        return result;
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

    private static string ScrubPropertyFromJson(string json, string propertyName)
    {
        var pattern = $"(\"{propertyName}\"\\s*:\\s*\")[^\"]*(\")";
        var result = Regex.Replace(json, pattern, $"$1Scrubbed{propertyName}$2");
        return result;
    }
}
