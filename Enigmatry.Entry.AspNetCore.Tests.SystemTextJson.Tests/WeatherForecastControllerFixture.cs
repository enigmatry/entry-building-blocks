﻿using Enigmatry.Entry.AspNetCore.Tests.SampleAppTests;
using Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Http;

namespace Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Tests;

[Category("integration")]
public class WeatherForecastControllerFixture : WeatherForecastControllerFixtureBase
{
    protected override async Task<T?> GetAsync<T>(HttpClient client, string uri) where T : default =>
        await client.GetAsync<T>(uri);

    protected override Task<HttpResponseMessage> PostWithoutResponseCheckAsync<T>(HttpClient client, string uri,
        T content) => client.PostWithoutResponseCheckAsync(uri, content);

    protected override T? DeserializeJson<T>(string content) where T : default => content.Deserialize<T>();
}
