## What this repository is

A collection of reusable .NET NuGet packages (`net10.0`) published by Enigmatry. Each package is an independent module; all share configuration via `Directory.Build.props` and `Directory.Packages.props`.

## Build, test, and pack

```powershell
# Build
dotnet build -c Release

# Run only the fast test categories (what CI runs)
dotnet test -c Release --filter "Category=unit|Category=smoke"

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
```

`Enigmatry.Entry.Core` must have zero dependencies on any other Entry module.

## Key conventions

### Entities
- Domain entities inherit `Entity` → `EntityWithTypedId<TId>` → `EntityWithGuidId` (auto-generates sequential GUIDs in the constructor).
- Domain events are `abstract record`s inheriting `DomainEvent` (which implements `INotification`).
- Events are raised via `AddDomainEvent()` inside the entity; `PublishDomainEventsInterceptor` dispatches them via MediatR on `SaveChangesAsync`.

### CQRS
- Commands implement `ICommand` (no return) or `ICommand<TResponse>`.
- Queries implement `IQuery<TResponse>`.
- Both wrap MediatR `IRequest`. `IBaseCommand` tags the handler for `CommandTransactionBehavior` (wraps in a DB transaction).

### MediatR pipeline
Registered behaviors (in order): `ValidationBehavior` → `LoggingBehavior`. Validation runs FluentValidation; failures throw `ValidationException` before the handler executes.

### EF DbContext
Extend `BaseDbContext`. Pass `EntitiesDbContextOptions` to the constructor — it controls which assembly is scanned for `IEntityTypeConfiguration` implementations.

### NuGet Central Package Management
All version numbers live in `Directory.Packages.props`. Never add a `Version` attribute to a `.csproj` file. Transitive pinning is enabled (`CentralPackageTransitivePinningEnabled=true`). Lock files are enforced (`RestorePackagesWithLockFile=true`).

### Build settings (`Directory.Build.props`)
- `TreatWarningsAsErrors=true` and `EnforceCodeStyleInBuild=true` — all warnings are build errors.
- `LangVersion=14.0`, `Nullable=enable`, `ImplicitUsings=enable`.
- MediatR is pinned to an exact version (`[12.4.1]`) to prevent accidental upgrades.

### Test conventions
- Test classes are named `*Fixture` or `*Tests`.
- Every test class carries `[Category("unit")]`, `[Category("integration")]`, or `[Category("smoke")]`.
- Only `unit` and `smoke` categories run in CI; `integration` tests (Azure services, Testcontainers) are skipped in CI.
- Assertions use **Shouldly** (`result.ShouldBe(...)`) or **Verify** for snapshot testing.
- Mocking uses **FakeItEasy** (`A.Fake<T>()`, `A.CallTo(...).MustHaveHappened(...)`).
- Shared test infrastructure lives in `Enigmatry.Entry.AspNetCore.Tests.Utilities`.

### SmartEnums
Use `Ardalis.SmartEnum` as the base. Companion packages add EF Core value converters (`SmartEnums.EntityFramework`), System.Text.Json converters (`SmartEnums.SystemTextJson`), and Swagger schema support (`SmartEnums.Swagger`).

## Versioning

Releases are versioned with [MinVer](https://github.com/adamralph/minver) — the version is derived from the nearest git tag. Install the CLI tool once:

```powershell
dotnet tool install --global minver-cli
```

NuGet packages are published automatically to NuGet.org by CI on `master` or `release/*` branches.
