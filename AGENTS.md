## What this repository is

A collection of reusable .NET NuGet packages (`net11.0`) published by Enigmatry. Each package is an independent module; all share configuration via `Directory.Build.props` and `Directory.Packages.props`.

## Build, test, and pack

```powershell
# Build
dotnet build -c Release

# Run only the fast test categories (what CI runs)
dotnet test -c Release --filter "TestCategory=unit|TestCategory=smoke"

# Run a single test project
dotnet test Enigmatry.Entry.MediatR.Tests -c Release

# Run a single test by name
dotnet test Enigmatry.Entry.MediatR.Tests -c Release --filter "FullyQualifiedName~ValidationBehaviorPipelineFixture"

# Full pipeline: build + test + pack (requires minver-cli installed globally)
.\build.ps1
```

After changing package versions in `Directory.Packages.props`, regenerate lock files:

```powershell
dotnet restore --force-evaluate
```

## Architecture overview

Every module follows this shape:

```
Enigmatry.Entry.<Module>/        # Production package (IsPackable=true)
Enigmatry.Entry.<Module>.Tests/  # Tests for that module
```

Packages form a layered dependency — upper layers depend on lower:

```
Enigmatry.Entry.Core                    ← base entities, CQRS interfaces, paging, domain events
Enigmatry.Entry.Infrastructure          ← TimeProvider implementation
Enigmatry.Entry.EntityFramework         ← BaseDbContext, repository, UoW, domain-event interceptor
Enigmatry.Entry.MediatR                 ← ValidationBehavior, LoggingBehavior, NullMediator
Enigmatry.Entry.AspNetCore              ← exception handling, HTTPS, transaction filter, action-result helpers
Enigmatry.Entry.AspNetCore.Authorization
Enigmatry.Entry.SmartEnums.*            ← SmartEnum + EF, Swagger, STJ, Verify helpers
Enigmatry.Entry.Csv / Email / BlobStorage / AzureSearch / GraphApi / HealthChecks / Scheduler / TemplatingEngine.*
Enigmatry.Entry.Core.EntityFramework    ← EF queryable extensions, data seeding
Enigmatry.Entry.Swagger                 ← NSwag OpenAPI setup and security
Enigmatry.Entry.Randomness              ← random data for tests and seeding
Enigmatry.Entry.AspNetCore.Tests.*      ← test packages for consumers: Utilities (Testcontainers SQL, Respawn, in-memory DB), NewtonsoftJson/SystemTextJson HttpClient helpers
```

`Enigmatry.Entry.Core` must have zero dependencies on any other Entry module.

Folder names match the package ID, except `Enigmatry.Entry.EmailClient` (package `Enigmatry.Entry.Email`) and `Enigmatry.Entry.SwaggerSecurity` (package `Enigmatry.Entry.Swagger`).

## Key conventions

### Entities
- Domain entities inherit `Entity` → `EntityWithTypedId<TId>` → `EntityWithGuidId` (auto-generates sequential GUIDs in the constructor).
- Domain events are `abstract record`s inheriting `DomainEvent` (which implements `INotification`).
- Events are raised via `AddDomainEvent()` inside the entity; `PublishDomainEventsInterceptor` dispatches them via MediatR on `SaveChangesAsync`.

### CQRS
- Commands implement `ICommand` or `ICommand<TResponse>`, queries `IQuery<TResponse>`; all are MediatR `IRequest`s, and commands share `IBaseCommand`.
- `IBaseCommand.TransactionBehavior` returns a `CommandTransactionBehavior` settings record (default: a `ReadCommitted` transaction; `NoTransaction` opts out). The abstract `SaveChangesBehavior<TDbContext, TRequest, TResponse>` in `Enigmatry.Entry.EntityFramework` reads it to wrap commands in a transaction and save; consumers subclass and register it.

### MediatR pipeline
`Enigmatry.Entry.MediatR` ships `ValidationBehavior` (runs FluentValidation; failures throw `ValidationException` before the handler) and `LoggingBehavior`. The packages register no behaviors: consumers do, as `ValidationBehavior` → `LoggingBehavior` (see the package README).

### EF DbContext
Extend `BaseDbContext`. Pass `EntitiesDbContextOptions` to the constructor — it controls which assembly is scanned for `IEntityTypeConfiguration` implementations.

### NuGet Central Package Management
All version numbers live in `Directory.Packages.props`. Never add a `Version` attribute to a `.csproj` file. Transitive pinning is enabled (`CentralPackageTransitivePinningEnabled=true`). Lock files are enforced (`RestorePackagesWithLockFile=true`), and CI restores in locked mode: a stale `packages.lock.json` fails the build with NU1004, so regenerate them with `dotnet restore --force-evaluate` after any version change. `nuget.config` sets a 24-hour minimum publish age on nuget.org: `dotnet package add/update` only pick versions at least that old, and an explicitly requested version bypasses it.

.NET 11 ships `Microsoft.Extensions.{Caching,Configuration,DependencyInjection,Diagnostics,FileProviders,Hosting,Logging}.Abstractions`, `Microsoft.Extensions.Options` and `Microsoft.Extensions.Primitives` in the shared framework. A `PackageReference` to any of them raises NU1510, which is a build error here.

### Build settings (`Directory.Build.props`)
- `TreatWarningsAsErrors=true` and `EnforceCodeStyleInBuild=true` — all warnings are build errors.
- `LangVersion=15.0`, `AnalysisLevel=11.0`, `Nullable=enable`, `ImplicitUsings=enable`.
- `global.json` pins the .NET 11 SDK (`11.0.100`, `rollForward: latestFeature`); both pipelines install the SDK from it.
- MediatR is pinned to an exact version (`[12.4.1]`) to prevent accidental upgrades.

### Code organization
- One type/class per file, file named after the type. Applies to every language (C#, TS, ...).

### Test conventions
- Test classes are named `*Fixture` or `*Tests`.
- Every test class carries `[Category("unit")]` or `[Category("integration")]`. CI runs `unit` (its filter also admits `smoke`).
- `integration` tests: `AzureSearch.Tests` needs an Azure AI Search service, configured through user secrets or the `AzureSearch__SearchServiceEndPoint` / `AzureSearch__ApiKey` env vars (the shared test service holds at most 15 indexes). The `AspNetCore.Tests*` fixtures host SampleApp in-process and need nothing external.
- Assertions use **Shouldly** (`result.ShouldBe(...)`) or **Verify** for snapshot testing.
- Verify snapshots (`*.verified.txt`) sit next to their fixture; `*.received.*` files are gitignored.
- Mocking uses **FakeItEasy** (`A.Fake<T>()`, `A.CallTo(...).MustHaveHappened(...)`).
- Shared test infrastructure for this repo lives in `Enigmatry.Entry.AspNetCore.Tests`. `Enigmatry.Entry.AspNetCore.Tests.Utilities` is a package for consumers.
- Detailed guidance: `.claude/skills/csharp-unit-tests` (tests) and `.claude/skills/azure-devops-pipelines` (pipeline YAML).

### SmartEnums
Use `Ardalis.SmartEnum` as the base. Companion packages add EF Core value converters (`SmartEnums.EntityFramework`), System.Text.Json converters (`SmartEnums.SystemTextJson`), and Swagger schema support (`SmartEnums.Swagger`).

## Versioning

Releases are versioned with [MinVer](https://github.com/adamralph/minver) — the version is derived from the nearest git tag. Install the CLI tool once:

```powershell
dotnet tool install --global minver-cli
```

NuGet packages are published automatically to NuGet.org by CI on `master` or `release/*` branches.
