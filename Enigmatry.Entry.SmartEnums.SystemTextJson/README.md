# Smart Enums System.Text.Json Integration

This library provides System.Text.Json integration for the Smart Enums feature, allowing proper serialization and deserialization of smart enum types.

## Intended Usage

Use this library when you need to serialize/deserialize Smart Enum types using System.Text.Json in your application.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.SmartEnums.SystemTextJson
```

## Usage Example

Register Smart Enum JSON converters in your application:

```csharp
using Enigmatry.Entry.SmartEnums.SystemTextJson;
using System.Reflection;
using System.Text.Json;

// Configure JSON options
var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

// Add Smart Enum converters for all SmartEnum types in specified assemblies
// Choose ValueConverter to serialize by the enum's numeric value
options.Converters.EntryAddSmartEnumJsonConverters(
    SmartEnumConverterType.ValueConverter, 
    new[] { Assembly.GetExecutingAssembly() });

// Use in serialization/deserialization
var json = JsonSerializer.Serialize(myObject, options);
var deserialized = JsonSerializer.Deserialize<MyClass>(json, options);
```

## Dependency Injection Example

When using ASP.NET Core:

```csharp
using Enigmatry.Entry.SmartEnums.SystemTextJson;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Add Smart Enum converters for all SmartEnum types in the current assembly
            // You can also use NameConverter to serialize by the enum's name
            options.JsonSerializerOptions.Converters.EntryAddSmartEnumJsonConverters(
                SmartEnumConverterType.NameConverter,
                new[] { Assembly.GetExecutingAssembly() });
        });
}
```
