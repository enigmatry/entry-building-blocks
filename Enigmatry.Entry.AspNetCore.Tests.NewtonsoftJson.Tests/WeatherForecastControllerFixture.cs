using Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Http;
using Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;

namespace Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Tests;

[Category("integration")]
public class WeatherForecastControllerFixture : WeatherForecastControllerFixtureBase
{
    public WeatherForecastControllerFixture()
    {
        UseNewtonsoftJson();
    }

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
    public async Task TestPost()
    {
        var response = await TestPostInvalidRequest();
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

    protected override async Task<HttpResponseMessage> PostWithoutResponseCheckAsync<T>(HttpClient client, string uri,
        T content) => await client.PostWithoutResponseCheckAsync<T>(uri, content);

    protected override T? DeserializeJson<T>(string content) where T : default => content.Deserialize<T>();
}
