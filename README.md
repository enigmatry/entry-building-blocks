# Entry building blocks

Entry building blocks are the .NET packages Enigmatry builds client systems on: persistence, security, search, storage, mail, scheduling, health checks and API documentation, shipped as ordinary NuGet packages rather than as a framework that takes over your application. Each package is an independent module on `net11.0`; take the ones you need and leave the rest.

Together with the [Entry blueprint](https://github.com/enigmatry/entry-blueprint), a ready-to-run solution skeleton, and [Entry code generation](https://github.com/enigmatry/entry-code-generation), which generates Angular forms and tables from C# configuration, they form Entry: the open-source foundation described at https://www.enigmatry.com/en/entry/.

## Packages

All packages are published on nuget.org under the [Enigmatry](https://www.nuget.org/profiles/Enigmatry) profile.

| Package | What it adds |
|---|---|
| `Enigmatry.Entry.Core` | Base entities, typed ids, CQRS interfaces, paging and domain events. Depends on no other Entry module. |
| `Enigmatry.Entry.Core.EntityFramework` | Core abstractions for projects that depend on Entity Framework. |
| `Enigmatry.Entry.Infrastructure` | Infrastructure services such as the `TimeProvider` implementation. |
| `Enigmatry.Entry.EntityFramework` | `BaseDbContext`, repository, unit of work and the domain-event interceptor for EF Core. |
| `Enigmatry.Entry.MediatR` | Validation and logging pipeline behaviours and a null mediator for MediatR. |
| `Enigmatry.Entry.AspNetCore` | Exception handling, HTTPS, transaction filter and action-result helpers for ASP.NET Core. |
| `Enigmatry.Entry.AspNetCore.Authorization` | Permission-based authorization for ASP.NET Core applications. |
| `Enigmatry.Entry.SwaggerSecurity` | Swagger documentation with security definitions. |
| `Enigmatry.Entry.SmartEnums` | SmartEnum support for Entry entities. |
| `Enigmatry.Entry.SmartEnums.EntityFramework` | SmartEnum value conversion for EF Core. |
| `Enigmatry.Entry.SmartEnums.SystemTextJson` | SmartEnum serialisation for System.Text.Json. |
| `Enigmatry.Entry.SmartEnums.Swagger` | SmartEnum schemas for Swagger and NSwag. |
| `Enigmatry.Entry.BlobStorage` | Azure Blob Storage access. |
| `Enigmatry.Entry.AzureSearch` | Azure AI Search (formerly Cognitive Search) indexing and querying. |
| `Enigmatry.Entry.GraphApi` | Microsoft Graph API client set-up. |
| `Enigmatry.Entry.EmailClient` | Sending e-mail. |
| `Enigmatry.Entry.TemplatingEngine.Razor` | Templating, for e-mail among other things, with Razor. |
| `Enigmatry.Entry.TemplatingEngine.Fluid` | Templating with Fluid (Liquid syntax). |
| `Enigmatry.Entry.Csv` | Reading and writing CSV. |
| `Enigmatry.Entry.HealthChecks` | Health-check registration and endpoints. |
| `Enigmatry.Entry.Scheduler` | Start-up extensions for a scheduler web job running Quartz jobs. |
| `Enigmatry.Entry.Randomness` | Random values for tests and seeding. |

The packages form layers; upper layers depend on lower ones, never the other way round:

```
Enigmatry.Entry.Core                    base entities, CQRS interfaces, paging, domain events
Enigmatry.Entry.Infrastructure          TimeProvider implementation
Enigmatry.Entry.EntityFramework         BaseDbContext, repository, unit of work, domain-event interceptor
Enigmatry.Entry.MediatR                 ValidationBehavior, LoggingBehavior, NullMediator
Enigmatry.Entry.AspNetCore              exception handling, HTTPS, transaction filter, action-result helpers
Enigmatry.Entry.AspNetCore.Authorization
Enigmatry.Entry.SmartEnums.*            SmartEnum + EF Core, Swagger, System.Text.Json, Verify helpers
Enigmatry.Entry.Csv / EmailClient / BlobStorage / AzureSearch / GraphApi / HealthChecks / Scheduler / TemplatingEngine.*
```

## Getting started

Add the packages you need to your project:

```powershell
dotnet add package Enigmatry.Entry.Core
dotnet add package Enigmatry.Entry.EntityFramework
dotnet add package Enigmatry.Entry.AspNetCore
```

Each module registers itself through extension methods on the service collection or the application builder; the XML documentation on those methods describes the options. For a complete application that already wires the blocks together, start from the [Entry blueprint](https://github.com/enigmatry/entry-blueprint) instead: `dotnet new install Enigmatry.Entry.Blueprint.Template` and then `dotnet new blueprint`.

## Building from source

Building needs the .NET 11 SDK (pinned in `global.json`).

```powershell
# Build
dotnet build -c Release

# The fast test categories, which is what CI runs
dotnet test -c Release --filter "TestCategory=unit|TestCategory=smoke"

# One test project
dotnet test Enigmatry.Entry.MediatR.Tests -c Release

# Build, test and pack (needs minver-cli installed globally)
.\build.ps1
```

Every module follows the same shape: `Enigmatry.Entry.<Module>/` holds the package, `Enigmatry.Entry.<Module>.Tests/` its tests. Package versions are managed centrally in `Directory.Packages.props`; after changing one, run `dotnet restore --force-evaluate` to regenerate the lock files. Warnings are errors, and code style is enforced in the build.

## Contributing

Issues and pull requests are welcome. Keep a change inside one module where you can, add or update the tests of that module, and run the fast test categories before opening the pull request.

The repository's [Claude Code](https://claude.com/claude-code) setup uses Enigmatry's shared `entry-blueprint` plugin; `AGENTS.md` describes how. Install it once per clone with `claude plugin install entry-blueprint@enigmatry --scope project`.

## Licence

Apache-2.0. See [LICENSE.md](https://github.com/enigmatry/entry-building-blocks/blob/master/LICENSE.md).

## About Enigmatry

Entry is the open-source foundation Enigmatry builds client systems on. Enigmatry is a software company from Rotterdam, founded in 2006, with teams in Belgrade and Novi Sad.

* How we use Entry for clients: https://www.enigmatry.com/en/entry/
* How we build, with agents under checks: https://www.enigmatry.com/en/how-we-build/

Apache-2.0 © Enigmatry
