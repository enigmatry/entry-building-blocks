using Argon;
using Azure.Search.Documents.Models;

namespace Enigmatry.Entry.AzureSearch.Tests.Setup;

// Not all properties of FacetResult class were shown in the VerifyTests received files.
public class FacetResultJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var token = JToken.FromObject(value);

        if (token.Type != JTokenType.Object)
        {
            token.WriteTo(writer);
        }
        else
        {
            if (value is FacetResult facetResult)
            {
                var o = (JObject)token;
                o.Add(new JProperty("FacetType", facetResult.FacetType.ToString()));
                o.Add(new JProperty("Count", facetResult.Count));
                o.WriteTo(writer);
            }
        }
    }

    public override object ReadJson(JsonReader reader, Type type, object? existingValue, JsonSerializer serializer) =>
        throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");

    public override bool CanRead => false;

    public override bool CanConvert(Type type) =>
        type == typeof(FacetResult);
}
