using JetBrains.Annotations;
using NSwag.Generation.AspNetCore;

namespace Enigmatry.Entry.SmartEnums.Swagger;

[PublicAPI]
public static class AspNetCoreOpenApiDocumentGeneratorSettingsExtensions
{
    /// <summary>
    /// Configure SmartEnums for NSwag. Adds SmartEnumSwaggerSchemaProcessor to SchemaProcessors
    /// </summary>
    /// <param name="settings"></param>
    public static void EntryConfigureSmartEnums(this AspNetCoreOpenApiDocumentGeneratorSettings settings) =>
        settings.SchemaSettings.SchemaProcessors.Add(new SmartEnumSwaggerSchemaProcessor());
}
