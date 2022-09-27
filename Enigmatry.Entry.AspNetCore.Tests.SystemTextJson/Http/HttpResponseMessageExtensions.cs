using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Http
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T?> DeserializeAsync<T>(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<T>(content);
        }

        public static async Task<T?> DeserializeWithStatusCodeCheckAsync<T>(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode
                ? Deserialize<T>(content)
                : throw DisposeResponseContentAndThrowException(response, content);
        }

        private static T? Deserialize<T>(string content) =>
            JsonSerializer.Deserialize<T>(content, HttpSerializationOptions.Options);

        public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _ = DisposeResponseContentAndThrowException(response, content);
            }
        }

        private static HttpOperationException DisposeResponseContentAndThrowException(HttpResponseMessage response,
            string content)
        {
            // Disposing the content should help users: If users call EnsureSuccessStatusCode(), an exception is
            // thrown if the statusCode status code is != 2xx. I.e. the behavior is similar to a failed request (e.g.
            // connection failure). Users are not expected to dispose the content in this case: If an exception is 
            // thrown, the object is responsible fore cleaning up its state.
            response.Content?.Dispose();
            throw new HttpOperationException(
                $"StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}, RequestUri: {response.RequestMessage.RequestUri}, Content: {content}.");
        }
    }
}
