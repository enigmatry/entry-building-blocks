using System.Net.Http;
using System.Threading.Tasks;

namespace Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Http;

public static class HttpResponseMessageExtensions
{
    public static async Task<T?> DeserializeAsync<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return content.Deserialize<T>();
    }

    public static async Task<T?> DeserializeWithStatusCodeCheckAsync<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return response.IsSuccessStatusCode
            ? content.Deserialize<T>()
            : throw DisposeResponseContentAndThrowException(response, content);
    }

    public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _ = DisposeResponseContentAndThrowException(response, content);
        }
    }

    private static HttpRequestException DisposeResponseContentAndThrowException(HttpResponseMessage response,
        string content)
    {
        // Disposing the content should help users: If users call EnsureSuccessStatusCode(), an exception is
        // thrown if the statusCode status code is != 2xx. I.e. the behavior is similar to a failed request (e.g.
        // connection failure). Users are not expected to dispose the content in this case: If an exception is 
        // thrown, the object is responsible for cleaning up its state.
        response.Content?.Dispose();
        throw new HttpRequestException(
            $"StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}, RequestUri: {response.RequestMessage?.RequestUri}, Content: {content}.");
    }
}
