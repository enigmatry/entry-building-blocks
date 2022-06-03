using System.Text.Json;

namespace Enigmatry.BuildingBlocks.AspNetCore.Tests.Http
{
    public static class HttpSerializationOptions
    {
        public static JsonSerializerOptions Options { get; set; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
