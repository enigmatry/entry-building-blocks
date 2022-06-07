using Newtonsoft.Json;

namespace Enigmatry.BuildingBlocks.AspNetCore.Tests.NewtonsoftJson.Http
{
    public static class HttpSerializationSettings
    {
        public static JsonSerializerSettings Settings { get; set; } = new();
    }
}
