# Smart Enums Swagger Integration

This library provides Swagger/OpenAPI integration for Smart Enums, enabling proper documentation of Smart Enum types in your API documentation.

## Intended Usage

Use this library when you want your Smart Enum types to be properly displayed and documented in Swagger UI, with correct schema generation and example values.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.SmartEnums.Swagger
```

## Usage Example

Configure SmartEnums in your NSwag OpenAPI documentation settings:

```csharp
using Enigmatry.Entry.SmartEnums.Swagger;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation.AspNetCore;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add OpenAPI/Swagger documentation with NSwag
        services.AddOpenApiDocument(document =>
        {
            document.Title = "My API";
            document.Version = "v1";
            
            // Configure Smart Enum schema processing for Swagger
            document.EntryConfigureSmartEnums();
        });
    }
}
```

With this configuration, Smart Enum properties in your API controllers will be correctly documented in Swagger UI, showing the available enum values and their descriptions.
