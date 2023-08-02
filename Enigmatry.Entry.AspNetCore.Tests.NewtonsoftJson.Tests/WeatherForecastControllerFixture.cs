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

    protected override async Task<T?> GetAsync<T>(HttpClient client, string uri) where T : default =>
        await client.GetAsync<T>(uri);

    protected override T? DeserializeJson<T>(string content) where T : default => content.Deserialize<T>();
}
