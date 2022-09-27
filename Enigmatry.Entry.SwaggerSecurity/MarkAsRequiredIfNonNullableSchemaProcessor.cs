using NJsonSchema;
using NJsonSchema.Generation;

namespace Enigmatry.Entry.Swagger
{
    /// <summary>
    /// This schema processor marks properties that are non nullable as required. Useful when nullable reference types are used.
    /// Based on: https://github.com/RicoSuter/NSwag/issues/3110
    /// </summary>
    internal class MarkAsRequiredIfNonNullableSchemaProcessor : ISchemaProcessor
    {
        public void Process(SchemaProcessorContext context)
        {
            foreach (var property in context.Schema.Properties.Values)
            {
                if (!property.IsNullable(SchemaType.OpenApi3))
                {
                    property.IsRequired = true;
                }
            }
        }
    }
}
