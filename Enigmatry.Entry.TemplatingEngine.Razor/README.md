# Razor Templating Engine

A templating engine library that uses the Razor view engine to generate content from templates.

## Intended Usage

Use this library when you need to generate dynamic content from templates using the familiar Razor syntax, such as for email templates, document generation, or dynamic content rendering.

## Templates

Templates are Razor views (`.cshtml`) compiled at build time:

- In a web project (`Microsoft.NET.Sdk.Web`), the views compile into the application.
- In a Razor class library (`Microsoft.NET.Sdk.Razor`), set `<AddRazorSupportForMvc>true</AddRazorSupportForMvc>` so the views compile for MVC, and reference the library from the application.

Reference a template by its app-relative path, including the extension: `~/EmailTemplates/WelcomeEmail.cshtml`. For templates that change without a rebuild, use `Enigmatry.Entry.TemplatingEngine.Fluid`.

The engine finds compiled views through MVC's application parts, so it runs inside an ASP.NET Core host: a web application, or a generic host with `ConfigureWebHostDefaults` for a background service or console application.

## Installation

```bash
dotnet add package Enigmatry.Entry.TemplatingEngine.Razor
```

## Registration

```csharp
services.AddRazorPages();
services.AddEntryTemplatingEngine();
```

## Usage

Inject `ITemplatingEngine` and render a template with a model, optionally passing view bag data:

```csharp
public class EmailService(ITemplatingEngine templatingEngine)
{
    public Task<string> GenerateWelcomeEmail(EmailModel model) =>
        templatingEngine.RenderFromFileAsync("~/EmailTemplates/WelcomeEmail.cshtml", model);

    public Task<string> GenerateWelcomeEmailWithTitle(EmailModel model, string title) =>
        templatingEngine.RenderFromFileAsync("~/EmailTemplates/WelcomeEmail.cshtml", model,
            new Dictionary<string, object> { ["Title"] = title });
}
```
