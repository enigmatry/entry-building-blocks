using Newtonsoft.Json;

namespace Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Http
{
    public static class HttpSerializationSettings
    {
        public static JsonSerializerSettings Settings { get; set; } = new();
    }
}
