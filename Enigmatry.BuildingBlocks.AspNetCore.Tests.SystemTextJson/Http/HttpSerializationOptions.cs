using System.Text.Json;

namespace Enigmatry.BuildingBlocks.AspNetCore.Tests.SystemTextJson.Http
{
    public static class HttpSerializationOptions
    {
        public static JsonSerializerOptions Options { get; set; } = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
