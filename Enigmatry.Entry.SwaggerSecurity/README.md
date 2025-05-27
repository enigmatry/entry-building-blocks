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
