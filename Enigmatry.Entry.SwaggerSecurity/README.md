# Swagger Security

This library provides security enhancements for Swagger/OpenAPI documentation in ASP.NET Core applications.

## Intended Usage

Use this library to add security features to your Swagger documentation, such as authentication requirements, API key configuration, and endpoint access control.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.Swagger
```

## Usage Example

```csharp
using Enigmatry.Entry.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation.AspNetCore;
using System.Collections.Generic;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add Swagger with OAuth 2.0 Authorization Code flow
        services.AddEntrySwaggerWithAuthorizationCode(
            appTitle: "My API",
            authorizationUrl: "https://auth.example.com/authorize",
            tokenUrl: "https://auth.example.com/token",
            scopes: new Dictionary<string, string>
            {
                { "api", "API access" },
                { "openid", "OpenID" },
                { "profile", "Profile information" }
            },
            appVersion: "v1",
            configureSettings: settings =>
            {
                // Configure additional Swagger document settings
                settings.Description = "API with OAuth2 security";
                
                // You can add custom schema processors if needed
                settings.SchemaSettings.SchemaNameGenerator = new CustomSwaggerSchemaNameGenerator();
            });
    }
    
    public void Configure(IApplicationBuilder app)
    {
        // Enable Swagger UI with OAuth2 client integration
        app.UseEntrySwaggerWithOAuth2Client(
            clientId: "swagger-ui-client",
            clientSecret: "",  // Optional client secret
            path: "/api-docs"
        );
    }
}
```

## Breaking change: Implicit Grant flow removed

`AddEntrySwaggerWithImplicitGrant` and the legacy `AppAddSwaggerWithImplicitGrant` have been removed.
The OAuth2 Implicit Grant flow is deprecated and removed in [OAuth 2.1](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-v2-1):
access tokens are returned in the browser URL fragment (leakable via referrer headers, browser history and server logs)
and no refresh tokens are issued.

Replace the call with the Authorization Code + PKCE equivalent; the parameters are identical:

```csharp
// Before
services.AddEntrySwaggerWithImplicitGrant(appTitle, authorizationUrl, tokenUrl, scopes, appVersion, configureSettings);

// After
services.AddEntrySwaggerWithAuthorizationCode(appTitle, authorizationUrl, tokenUrl, scopes, appVersion, configureSettings);
```

and wire the Swagger UI with `app.UseEntrySwaggerWithOAuth2Client(clientId, clientSecret, path)`, which enables PKCE
for the Authorization Code grant. Your identity provider must allow the Authorization Code flow (with PKCE, no client
secret) for the Swagger UI client.
