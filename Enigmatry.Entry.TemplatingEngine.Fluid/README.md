# Fluid template building block

Building Block with startup extensions for a fluid templating engine.

## Registration

You can use the `AddFluidTemplateEngine` extension method on `IServiceCollection` to add a fluid template engine to the IoC container. 
This extension method will also configure the fluid template engine and fluid template provider:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddFluidTemplateEngine();
}
```

## FluidTemplateEngineOptions

Optionally, you can also register custom options for the fluid template engine:

```csharp
var options = new FluidTemplateEngineOptions
{
    MemberNameStrategy = MemberNameStrategies.SnakeCase,
    CultureInfo = CultureInfo.GetCultureInfo("nl-NL"),
    TimeZoneInfo = TimeZoneInfo.Utc,
    ValueConverters =
    [
        value => value is Enum e ? new StringValue(e.GetDescription()) : null,
        value => value is DateTimeOffset dateTime
            ? TimeZoneInfo.ConvertTimeFromUtc(dateTime.UtcDateTime.ToUniversalTime(), TimeZoneInfo.Utc)
                .ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.GetCultureInfo("nl-NL"))
            : null
    ]
};
services.AddSingleton(Options.Create(options));
```

Default values are:
```csharp
var options = new FluidTemplateEngineOptions
{
    MemberNameStrategy = MemberNameStrategies.CamelCase,
    CultureInfo = CultureInfo.InvariantCulture,
    TimeZoneInfo = TimeZoneInfo.Local,
    ValueConverters = new List<Func<object, object>>()
};
```