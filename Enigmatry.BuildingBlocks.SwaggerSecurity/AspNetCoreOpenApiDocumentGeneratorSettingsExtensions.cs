using System;
using JetBrains.Annotations;
using NSwag.Generation.AspNetCore;

namespace Enigmatry.BuildingBlocks.Swagger
{
    public static class AspNetCoreOpenApiDocumentGeneratorSettingsExtensions
    {
        internal static void ConfigureSwaggerSettings(this AspNetCoreOpenApiDocumentGeneratorSettings settings,
            string appTitle, string appVersion, Action<AspNetCoreOpenApiDocumentGeneratorSettings>? configureSettings)
        {
            settings.DocumentName = appVersion;
            settings.Title = appTitle;
            settings.Version = appVersion;
            settings.SchemaNameGenerator = new CustomSwaggerSchemaNameGenerator();

            configureSettings?.Invoke(settings);
        }

        [PublicAPI]
        public static void
            MarkNonNullablePropertiesAsRequired(this AspNetCoreOpenApiDocumentGeneratorSettings settings) =>
            settings.SchemaProcessors.Add(new MarkAsRequiredIfNonNullableSchemaProcessor());
    }
}
