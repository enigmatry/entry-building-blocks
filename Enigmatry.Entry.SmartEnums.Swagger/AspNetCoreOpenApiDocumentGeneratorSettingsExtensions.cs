using JetBrains.Annotations;
using NSwag.Generation.AspNetCore;

namespace Enigmatry.Entry.SmartEnums.Swagger;

[PublicAPI]
public static class AspNetCoreOpenApiDocumentGeneratorSettingsExtensions
{
    public static void EntryConfigureSmartEnums(this AspNetCoreOpenApiDocumentGeneratorSettings settings) =>
        settings.SchemaSettings.SchemaProcessors.Add(new SmartEnumSwaggerSchemaProcessor());
}
