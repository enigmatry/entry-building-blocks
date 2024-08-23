using System.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Enigmatry.Entry.SmartEnums.Swagger;

/// <summary>
/// Generate schema for SmartEnum type. Value is integer, enumeration is values of SmartEnum
/// </summary>
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

            var smartEnumValues = type.GetSmartEnumValues();

            foreach (var smartEnum in smartEnumValues)
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
