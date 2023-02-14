using Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Http;
using Newtonsoft.Json;

namespace Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson;
internal static class StringExtensions
{
    internal static T? Deserialize<T>(this string content) =>
        JsonConvert.DeserializeObject<T>(content, HttpSerializationSettings.Settings);
}
