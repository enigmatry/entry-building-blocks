# Fluid templating engine

Renders [Liquid](https://shopify.github.io/liquid/) templates with [Fluid](https://github.com/sebastienros/fluid), for example for email bodies.

## Registration

```csharp
services.AddLiquidTemplatingEngine(options =>
{
    options.MemberNameStrategy = MemberNameStrategies.SnakeCase;
    options.CultureInfo = CultureInfo.GetCultureInfo("nl-NL");
    options.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");
});
```

Then inject `ITemplatingEngine` and render a template against a model:

```csharp
var body = await templatingEngine.RenderAsync("Hello {{ full_name }}", new { FullName = "Jane Doe" });
```

## Options

| Option | Default | Effect |
|---|---|---|
| `MemberNameStrategy` | `MemberNameStrategies.CamelCase` | How model members are named in templates (`snake_case` above) |
| `CultureInfo` | `CultureInfo.CurrentCulture` | Culture for numbers and dates |
| `TimeZoneInfo` | `TimeZoneInfo.Local` | Time zone that `DateTime` and `DateTimeOffset` values are converted to |

## Value conversion

The engine registers these converters:

- **Enums** render their `[Description]`.
- **`DateTime` and `DateTimeOffset`** are converted to `TimeZoneInfo` and formatted with `CultureInfo`.
- **`double` and `float`** render at the 15 and 7 significant digits they carry, so `1.23` renders as `1.23`. `NaN`, infinities and values beyond `decimal`'s range render as text.

To add your own, implement `IFluidValueConverter` and register it as a scoped service **after** calling `AddLiquidTemplatingEngine`. Converters registered later run first, and the first one that returns a non-null value wins.

## Filters

`format_number` formats a number with a .NET format string, in `CultureInfo` or in a culture you pass. A null value renders as an empty string:

```liquid
{{ record.amount | format_number: 'n2' }}
{{ record.amount | format_number: 'c2', 'en-US' }}
```

To add your own filter, implement `IFluidFilter` and register it as a scoped service.
