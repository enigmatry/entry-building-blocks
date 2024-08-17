using System.Reflection;
using Ardalis.SmartEnum;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Enigmatry.Entry.SmartEnums.Swagger;

internal class SmartEnumSwaggerSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var contextualType = context.ContextualType;
        var schema = context.Schema;
        var type = contextualType.OriginalType;

        if (type.IsDerivedFromSmartEnum())
        {
            schema.Items.Clear();
            schema.AllOf.Clear();
            schema.Type = JsonObjectType.Integer;
            schema.Enumeration.Clear();
            schema.EnumerationNames.Clear();
            schema.Properties.Clear();

            if (type.TryGetValues(out IEnumerable<object> values))
            {
                foreach (var smartEnum in values)
                {
                    var valuePropertyInfo = type.GetRuntimeProperty("Value")!;
                    var namePropertyInfo = type.GetRuntimeProperty("Name")!;
                    var value = valuePropertyInfo.GetValue(smartEnum)!;
                    var name = (string)namePropertyInfo.GetValue(smartEnum)!;

                    schema.Enumeration.Add(value);
                    schema.EnumerationNames.Add(name);
                }
            }
        }
    }
}
