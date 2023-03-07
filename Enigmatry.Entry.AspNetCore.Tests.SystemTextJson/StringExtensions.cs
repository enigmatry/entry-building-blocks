using Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Http;
using System.Text.Json;

namespace Enigmatry.Entry.AspNetCore.Tests.SystemTextJson;
internal static class StringExtensions
{
    internal static T? Deserialize<T>(this string content) =>
        JsonSerializer.Deserialize<T>(content, HttpSerializationOptions.Options);
}
