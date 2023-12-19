using System;
using JetBrains.Annotations;
using NSwag.Generation.AspNetCore;

namespace Enigmatry.Entry.Swagger
{
    public static class AspNetCoreOpenApiDocumentGeneratorSettingsExtensions
    {
        internal static void ConfigureSwaggerSettings(this AspNetCoreOpenApiDocumentGeneratorSettings settings,
            string appTitle, string appVersion, Action<AspNetCoreOpenApiDocumentGeneratorSettings>? configureSettings)
        {
            settings.DocumentName = appVersion;
            settings.Title = appTitle;
            settings.Version = appVersion;
            settings.SchemaSettings.SchemaNameGenerator = new CustomSwaggerSchemaNameGenerator();

            configureSettings?.Invoke(settings);
        }

        [PublicAPI]
        public static void
            MarkNonNullablePropertiesAsRequired(this AspNetCoreOpenApiDocumentGeneratorSettings settings) =>
            settings.SchemaSettings.SchemaProcessors.Add(new MarkAsRequiredIfNonNullableSchemaProcessor());
    }
}
