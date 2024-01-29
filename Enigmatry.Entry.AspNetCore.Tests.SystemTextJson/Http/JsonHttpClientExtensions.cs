using Enigmatry.Entry.AspNetCore.Tests.Utilities.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Http;

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
        var response = await client.PutAsJsonAsync(uri, content, HttpSerializationOptions.Options);
        await response.EnsureSuccessStatusCodeAsync();
    }

    public static async Task<TResponse?> PutAsync<T, TResponse>(this HttpClient client, string uri, T content)
    {
        var response = await client.PutAsJsonAsync(uri, content, HttpSerializationOptions.Options);
        return await response.DeserializeWithStatusCodeCheckAsync<TResponse>();
    }

    public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, string uri, T content)
    {
        var response = await client.PostAsJsonAsync(uri, content, HttpSerializationOptions.Options);
        await response.EnsureSuccessStatusCodeAsync();
        return response;
    }

    public static async Task<TResponse?> PostAsync<T, TResponse>(this HttpClient client, string uri, T content)
    {
        var response = await client.PostAsJsonAsync(uri, content, HttpSerializationOptions.Options);
        return await response.DeserializeWithStatusCodeCheckAsync<TResponse>();
    }

    public static async Task<HttpResponseMessage> PostWithoutResponseCheckAsync<T>(this HttpClient client, string uri,
        T content)
    {
        var response = await client.PostAsJsonAsync(uri, content, HttpSerializationOptions.Options);
        return response;
    }
}
