using System;
using NSwag.Generation.AspNetCore;

namespace Enigmatry.BuildingBlocks.Swagger
{
    internal static class AspNetCoreOpenApiDocumentGeneratorSettingsExtensions
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

        public static AspNetCoreOpenApiDocumentGeneratorSettings MarkNonNullablePropertiesAsRequired(
            this AspNetCoreOpenApiDocumentGeneratorSettings settings, bool enable)
        {
            if (enable)
            {
                settings.SchemaProcessors.Add(new MarkAsRequiredIfNonNullableSchemaProcessor());
            }

            return settings;
        }
    }
}
