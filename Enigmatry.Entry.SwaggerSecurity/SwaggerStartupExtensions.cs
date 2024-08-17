using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace Enigmatry.Entry.Swagger;

public static class SwaggerStartupExtensions
{
    [Obsolete("Use UseEntrySwagger instead")]
    public static void AppUseSwagger(this IApplicationBuilder app, string path = "") => app.UseEntrySwagger(path);

    /// <summary>
    ///     Add the OpenAPI/Swagger middleware to the Asp.Net Core pipeline.
    /// </summary>
    /// <param name="app">The application to configure</param>
    /// <param name="path">The internal swagger route (must start with '/')</param>
    public static void UseEntrySwagger(this IApplicationBuilder app, string path = "")
    {
        app.UseOpenApi();
        app.UseSwaggerUi(c => c.Path = path);
    }

    [Obsolete("Use UseEntrySwaggerWithOAuth2Client instead")]
    public static void AppUseSwaggerWithOAuth2Client(this IApplicationBuilder app, string clientId,
        string clientSecret = "", string path = "") =>
        app.UseEntrySwaggerWithOAuth2Client(clientId, clientSecret, path);

    /// <summary>
    ///     Add the OpenAPI/Swagger middleware to the Asp.Net Core pipeline, and configure the OAuth2 Client.
    /// </summary>
    /// <param name="app">The application to configure</param>
    /// <param name="clientId">The Client Id used by the OAuth2 Client</param>
    /// <param name="clientSecret">The optional Client Secret used by the OAuth2 Client</param>
    /// <param name="path">The internal swagger route (must start with '/')</param>
    public static void UseEntrySwaggerWithOAuth2Client(this IApplicationBuilder app, string clientId,
        string clientSecret = "", string path = "")
    {
        app.UseOpenApi();
        app.UseSwaggerUi(options =>
        {
            options.Path = path;
            if (!string.IsNullOrEmpty(clientId))
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    UsePkceWithAuthorizationCodeGrant = true
                };
            }
        });
    }

    [Obsolete("Use AddEntrySwagger instead")]
    public static void AppAddSwagger(this IServiceCollection services, string appTitle, string appVersion = "v1",
        Action<AspNetCoreOpenApiDocumentGeneratorSettings>? configureSettings = null) =>
        services.AddEntrySwagger(appTitle, appVersion, configureSettings);

    /// <summary>
    ///     Adds services required for OpenAPI 3.0 generation
    /// </summary>
    /// <param name="services">The container to which to register Swagger services</param>
    /// <param name="appTitle">The title of th application</param>
    /// <param name="appVersion">The version of the application</param>
    /// <param name="configureSettings">
    ///     Action to configure the OpenApi document generator settings after the initial settings
    ///     have been configured.
    /// </param>
    public static void AddEntrySwagger(this IServiceCollection services, string appTitle, string appVersion = "v1",
        Action<AspNetCoreOpenApiDocumentGeneratorSettings>? configureSettings = null) =>
        services.AddOpenApiDocument(settings =>
            settings.ConfigureSwaggerSettings(appTitle, appVersion, configureSettings));

    [Obsolete("Use AddEntrySwaggerWithAuthorizationCode instead")]
    public static void AppAddSwaggerWithAuthorizationCode(
        this IServiceCollection services,
        string appTitle,
        string authorizationUrl,
        string tokenUrl,
        Dictionary<string, string> scopes,
        string appVersion = "v1",
        Action<AspNetCoreOpenApiDocumentGeneratorSettings>? configureSettings = null) =>
        services.AddEntrySwaggerWithAuthorizationCode(appTitle, authorizationUrl, tokenUrl, scopes, appVersion,
            configureSettings);

    /// <summary>
    ///     Adds services required for OpenAPI 3.0 generation, and appends the OAuth2 security scheme and requirement to the
    ///     document's security
    ///     definitions (specifically for the Authorization Code flow).
    /// </summary>
    /// <param name="services">The container to which to register Swagger services</param>
    /// <param name="appTitle">The title of th application</param>
    /// <param name="authorizationUrl">The OAuth2 Authorization Url</param>
    /// <param name="tokenUrl">The OAuth2 Token Url</param>
    /// <param name="scopes">The available OAuth2 Scopes</param>
    /// <param name="appVersion">The version of the application</param>
    /// <param name="configureSettings">
    ///     Action to configure the OpenApi document generator settings after the initial settings
    ///     have been configured.
    /// </param>
    public static void AddEntrySwaggerWithAuthorizationCode(
        this IServiceCollection services,
        string appTitle,
        string authorizationUrl,
        string tokenUrl,
        Dictionary<string, string> scopes,
        string appVersion = "v1",
        Action<AspNetCoreOpenApiDocumentGeneratorSettings>? configureSettings = null)
    {
        if (string.IsNullOrEmpty(authorizationUrl))
        {
            throw new ArgumentException("Authorization URL cannot be empty", nameof(authorizationUrl));
        }

        if (string.IsNullOrEmpty(tokenUrl))
        {
            throw new ArgumentException("Token URL cannot be empty", nameof(tokenUrl));
        }

        if (scopes == null)
        {
            throw new ArgumentNullException(nameof(scopes), "Scopes cannot be null");
        }

        services.AddOpenApiDocument(settings =>
        {
            settings.ConfigureSwaggerSettings(appTitle, appVersion, configureSettings);
            settings.AddSecurity("oauth2",
                new OpenApiSecurityScheme
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

    [Obsolete("Use AddEntrySwaggerWithImplicitGrant instead")]
    public static void AppAddSwaggerWithImplicitGrant(
        this IServiceCollection services,
        string appTitle,
        string authorizationUrl,
        string tokenUrl,
        Dictionary<string, string> scopes,
        string appVersion = "v1",
        Action<AspNetCoreOpenApiDocumentGeneratorSettings>? configureSettings = null) =>
        services.AddEntrySwaggerWithImplicitGrant(appTitle, authorizationUrl, tokenUrl, scopes, appVersion,
            configureSettings);

    /// <summary>
    ///     Adds services required for OpenAPI 3.0 generation, and appends the OAuth2 security scheme and requirement to the
    ///     document's security
    ///     definitions (specifically for the Implicit Grant flow).
    /// </summary>
    /// <param name="services">The container to which to register Swagger services</param>
    /// <param name="appTitle">The title of th application</param>
    /// <param name="authorizationUrl">The OAuth2 Authorization Url</param>
    /// <param name="tokenUrl">The OAuth2 Token Url</param>
    /// <param name="scopes">The available OAuth2 Scopes</param>
    /// <param name="appVersion">The version of the application</param>
    /// <param name="configureSettings">
    ///     Action to configure the OpenApi document generator settings after the initial settings
    ///     have been configured.
    /// </param>
    public static void AddEntrySwaggerWithImplicitGrant(
        this IServiceCollection services,
        string appTitle,
        string authorizationUrl,
        string tokenUrl,
        Dictionary<string, string> scopes,
        string appVersion = "v1",
        Action<AspNetCoreOpenApiDocumentGeneratorSettings>? configureSettings = null)
    {
        if (string.IsNullOrEmpty(authorizationUrl))
        {
            throw new ArgumentException("Authorization URL cannot be empty", nameof(authorizationUrl));
        }

        if (string.IsNullOrEmpty(tokenUrl))
        {
            throw new ArgumentException("Token URL cannot be empty", nameof(tokenUrl));
        }

        if (scopes == null)
        {
            throw new ArgumentNullException(nameof(scopes), "Scopes cannot be null");
        }

        services.AddOpenApiDocument(settings =>
        {
            settings.ConfigureSwaggerSettings(appTitle, appVersion, configureSettings);
            settings.AddSecurity("oauth2",
                new OpenApiSecurityScheme
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
}
