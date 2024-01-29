using Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;
using Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Http;

namespace Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Tests;

[Category("integration")]
public class WeatherForecastControllerFixture : WeatherForecastControllerFixtureBase
{
    [Test]
    public async Task TestGet()
    {
        var response = base.GetAsync();
        await Verify(response);
    }

    [Test]
    public new Task TestGetError() => base.TestGetError();

    [Test]
    public new Task TestGetNotFoundError() => base.TestGetNotFoundError();

    [Test]
    public new Task TestGetNotFound() => base.TestGetNotFound();

    [Test]
    public async Task TestGetValidationError()
    {
        var response = await TestGetValidationErrorsAsProblemDetails();
        var details = await VerifyProblemDetailsAsResponse(response);
        await VerifyJson(details.responseJson);
    }

    [Test]
    public new async Task TestPostInvalidRequest()
    {
        var response = await base.TestPostInvalidRequest();
        var details = await VerifyProblemDetailsAsResponse(response);
        await VerifyJson(details.responseJson);
    }

    [Test]
    public async Task TestPostWithIncompatibleRequest()
    {
        var response = await TestPostIncompatibleRequestAsync();
        var details = await VerifyProblemDetailsAsResponse(response);
        await VerifyJson(details.responseJson);
    }

    protected override async Task<T?> GetAsync<T>(HttpClient client, string uri) where T : default =>
        await client.GetAsync<T>(uri);

    protected override Task<HttpResponseMessage> PostWithoutResponseCheckAsync<T>(HttpClient client, string uri,
        T content) => client.PostWithoutResponseCheckAsync(uri, content);

    protected override T? DeserializeJson<T>(string content) where T : default => content.Deserialize<T>();
}
