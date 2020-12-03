using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.Generic;

namespace Enigmatry.Blueprint.BuildingBlocks.Swagger
{
    public static class SwaggerStartupExtensions
    {
        /// <summary>
        /// Add the OpenAPI/Swagger middleware to the Asp.Net Core pipeline.
        /// </summary>
        /// <param name="path">The internal swagger route (must start with '/')</param>
        public static void AppUseSwagger(this IApplicationBuilder app, string path = "")
        {
            app.UseOpenApi();
            app.UseSwaggerUi3(c => c.Path = path);
        }

        /// <summary>
        /// Add the OpenAPI/Swagger middleware to the Asp.Net Core pipeline, and configure the OAuth2 Client.
        /// </summary>
        /// <param name="clientId">The Client Id used by the OAuth2 Client</param>
        /// <param name="clientSecret">The optional Client Secret used by the OAuth2 Client</param>
        /// <param name="path">The internal swagger route (must start with '/')</param>
        public static void AppUseSwaggerWithOAuth2Client(this IApplicationBuilder app, string clientId, string clientSecret = "", string path = "")
        {
            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.Path = path;
                if (!String.IsNullOrEmpty(clientId))
                {
                    options.OAuth2Client = new NSwag.AspNetCore.OAuth2ClientSettings()
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        UsePkceWithAuthorizationCodeGrant = true
                    };
                }
            });
        }

        /// <summary>
        /// Adds services required for OpenAPI 3.0 generation
        /// </summary>
        /// <param name="appTitle">The title of th application</param>
        /// <param name="appVersion">The version of the application</param>
        public static void AppAddSwagger(this IServiceCollection services, string appTitle, string appVersion = "v1")
        {
            services.AddOpenApiDocument(settings => settings.SetBasicSwaggerSettings(appTitle, appVersion));
        }

        /// <summary>
        /// Adds services required for OpenAPI 3.0 generation, and appends the OAuth2 security scheme and requirement to the document's security
        /// definitions (specifically for the Authorization Code flow).
        /// </summary>
        /// <param name="appTitle">The title of th application</param>
        /// <param name="authorizationUrl">The OAuth2 Authorization Url</param>
        /// <param name="tokenUrl">The OAuth2 Token Url</param>
        /// <param name="scopes">The availbale OAuth2 Scopes</param>
        /// <param name="appVersion">The version of the application</param>
        public static void AppAddSwaggerWithAuthorizationCode(
            this IServiceCollection services,
            string appTitle,
            string authorizationUrl,
            string tokenUrl,
            Dictionary<string, string> scopes,
            string appVersion = "v1")
        {
            if (String.IsNullOrEmpty(authorizationUrl))
                throw new ArgumentException("Authorization URL cannot be empty", nameof(authorizationUrl));
            if (String.IsNullOrEmpty(tokenUrl))
                throw new ArgumentException("Token URL cannot be empty", nameof(tokenUrl));
            if (scopes == null)
                throw new ArgumentNullException(nameof(scopes), "Scopes cannot be null");

            services.AddOpenApiDocument(settings =>
            {
                settings.SetBasicSwaggerSettings(appTitle, appVersion);
                settings.AddSecurity("oauth2", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = authorizationUrl,
                            TokenUrl = tokenUrl,
                            Scopes = scopes
                        }
                    }
                });
                settings.OperationProcessors.Add(new OperationSecurityScopeProcessor("oauth2"));
            });
        }

        /// <summary>
        /// Adds services required for OpenAPI 3.0 generation, and appends the OAuth2 security scheme and requirement to the document's security
        /// definitions (specifically for the Implicit Grant flow).
        /// </summary>
        /// <param name="appTitle">The title of th application</param>
        /// <param name="authorizationUrl">The OAuth2 Authorization Url</param>
        /// <param name="tokenUrl">The OAuth2 Token Url</param>
        /// <param name="scopes">The availbale OAuth2 Scopes</param>
        /// <param name="appVersion">The version of the application</param>
        public static void AppAddSwaggerWithImplicitGrant(
            this IServiceCollection services,
            string appTitle,
            string authorizationUrl,
            string tokenUrl,
            Dictionary<string, string> scopes,
            string appVersion = "v1")
        {
            if (String.IsNullOrEmpty(authorizationUrl))
                throw new ArgumentException("Authorization URL cannot be empty", nameof(authorizationUrl));
            if (String.IsNullOrEmpty(tokenUrl))
                throw new ArgumentException("Token URL cannot be empty", nameof(tokenUrl));
            if (scopes == null)
                throw new ArgumentNullException(nameof(scopes), "Scopes cannot be null");

            services.AddOpenApiDocument(settings =>
            {
                settings.SetBasicSwaggerSettings(appTitle, appVersion);
                settings.AddSecurity("oauth2", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = authorizationUrl,
                            TokenUrl = tokenUrl,
                            Scopes = scopes
                        }
                    }
                });
                settings.OperationProcessors.Add(new OperationSecurityScopeProcessor("oauth2"));
            });
        }

        private static void SetBasicSwaggerSettings(this AspNetCoreOpenApiDocumentGeneratorSettings settings, string appTitle, string appVersion)
        {
            settings.DocumentName = appVersion;
            settings.Title = appTitle;
            settings.Version = appVersion;
            settings.SchemaNameGenerator = new CustomSwaggerSchemaNameGenerator();
        }
    }
}
