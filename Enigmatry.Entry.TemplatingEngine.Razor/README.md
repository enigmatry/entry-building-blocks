# Razor Templating Engine

A templating engine library that uses the Razor view engine to generate content from templates.

## Intended Usage

Use this library when you need to generate dynamic content from templates using the familiar Razor syntax, such as for email templates, document generation, or dynamic content rendering.

## Templates

Templates are Razor views (`.cshtml`) compiled at build time, so the project that holds them uses the Razor SDK (`Microsoft.NET.Sdk.Web` or `Microsoft.NET.Sdk.Razor`). Reference a template by its app-relative path, including the extension: `~/EmailTemplates/WelcomeEmail.cshtml`. For templates that change without a rebuild, use `Enigmatry.Entry.TemplatingEngine.Fluid`.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.TemplatingEngine.Razor
```

## Usage Example

```csharp
using Enigmatry.Entry.TemplatingEngine;
using Microsoft.Extensions.DependencyInjection;

// In a non-DI environment, you would need to manually set up the service provider:
var services = new ServiceCollection();
services.AddRazorPages();
services.AddMvc();
services.AddEntryTemplatingEngine();
var serviceProvider = services.BuildServiceProvider();
var templateEngine = serviceProvider.GetRequiredService<ITemplatingEngine>();

// Define a model
var model = new EmailModel
{
    Username = "John",
    ActivationLink = "https://example.com/activate?token=abc123"
};

// Render a template from a file
string emailContent = await templateEngine.RenderFromFileAsync("~/EmailTemplates/WelcomeEmail.cshtml", model);

// You can also pass view bag data if needed
var viewBag = new Dictionary<string, object>
{
    { "CurrentDate", DateTime.Now }
};
string emailContent = await templateEngine.RenderFromFileAsync("~/EmailTemplates/WelcomeEmail.cshtml", model, viewBag);
```

## Dependency Injection Example

Register the template engine in your application's service collection:

```csharp
using Enigmatry.Entry.TemplatingEngine;

public void ConfigureServices(IServiceCollection services)
{
    // Register Razor pages and MVC components needed by the template engine
    services.AddRazorPages();
    services.AddMvc();
    
    // Register the templating engine
    services.AddEntryTemplatingEngine();
}
```

Then inject and use the template engine in your services:

```csharp
using Enigmatry.Entry.TemplatingEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmailService
{
    private readonly ITemplatingEngine _templateEngine;
    
    public EmailService(ITemplatingEngine templateEngine)
    {
        _templateEngine = templateEngine;
    }
    
    public async Task<string> GenerateWelcomeEmail(string username, string activationLink)
    {
        var model = new EmailModel
        {
            Username = username,
            ActivationLink = activationLink
        };
        return await _templateEngine.RenderFromFileAsync("~/Emails/Welcome.cshtml", model);
    }
}
```
