using Enigmatry.Entry.AspNetCore.Tests.Utilities.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Http;

public static class JsonHttpClientExtensions
{
    public static async Task<T?> GetAsync<T>(this HttpClient client, string uri)
    {
        var response = await client.GetAsync(uri);
        return await response.DeserializeWithStatusCodeCheckAsync<T>();
    }

    public static async Task<T?> GetAsync<T>(this HttpClient client, Uri uri,
        params KeyValuePair<string, string>[] parameters)
    {
        var resourceUri = uri.AppendParameters(parameters);

        var response = await client.GetAsync(resourceUri);
        return await response.DeserializeWithStatusCodeCheckAsync<T>();
    }

    public static async Task PutAsync<T>(this HttpClient client, string uri, T content)
    {
        var response = await client.PutAsync(uri, content, CreateFormatter());
        await response.EnsureSuccessStatusCodeAsync();
    }

    public static async Task<TResponse?> PutAsync<T, TResponse>(this HttpClient client, string uri, T content)
    {
        var response = await client.PutAsync(uri, content, CreateFormatter());
        return await response.DeserializeWithStatusCodeCheckAsync<TResponse>();
    }

    public static async Task PostAsync<T>(this HttpClient client, string uri, T content)
    {
        var response = await client.PostAsync(uri, content, CreateFormatter());
        await response.EnsureSuccessStatusCodeAsync();
    }

    public static async Task<TResponse?> PostAsync<T, TResponse>(this HttpClient client, string uri, T content)
    {
        var response = await client.PostAsync(uri, content, CreateFormatter());
        return await response.DeserializeWithStatusCodeCheckAsync<TResponse>();
    }

    private static JsonMediaTypeFormatter CreateFormatter() =>
        new()
        {
            SerializerSettings = HttpSerializationSettings.Settings
        };
}
