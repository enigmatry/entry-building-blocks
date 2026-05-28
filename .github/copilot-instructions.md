# Copilot Instructions — entry-building-blocks

## What this repository is

A collection of reusable .NET NuGet packages (targeting `net10.0`) published by Enigmatry. Each package is an independent module in its own project folder; all packages share configuration via `Directory.Build.props` and `Directory.Packages.props`.

## Build, test, and pack

```powershell
# Build (Release)
dotnet build -c Release

# Run only the fast test categories (what CI runs)
dotnet test -c Release --filter "Category=unit|Category=smoke"

# Run a single test project
dotnet test Enigmatry.Entry.MediatR.Tests -c Release

# Run a single test by name
dotnet test Enigmatry.Entry.MediatR.Tests -c Release --filter "FullyQualifiedName~ValidationBehaviorPipelineFixture"

# Build + test + pack (full pipeline, requires minver-cli installed globally)
.\build.ps1
```

NuGet lock files are enforced (`RestorePackagesWithLockFile=true`). After changing package versions in `Directory.Packages.props`, regenerate locks:

```powershell
dotnet restore --force-evaluate
```

## Versioning

Releases are versioned with [MinVer](https://github.com/adamralph/minver). The version is derived from the nearest git tag. Install the CLI tool once:

```powershell
dotnet tool install --global minver-cli
```

## Architecture overview

Every module follows the same shape:

```
Enigmatry.Entry.<Module>/          # Production package (IsPackable=true)
Enigmatry.Entry.<Module>.Tests/    # Tests for that module
```

The packages form a layered dependency:

```
Enigmatry.Entry.Core               ← base entities, CQRS interfaces, paging, domain events
Enigmatry.Entry.Infrastructure     ← TimeProvider implementation
Enigmatry.Entry.EntityFramework    ← BaseDbContext, repository, UoW, domain-event interceptor
Enigmatry.Entry.MediatR            ← ValidationBehavior, LoggingBehavior, NullMediator
Enigmatry.Entry.AspNetCore         ← exception handling, HTTPS, transaction filter, action-result helpers
Enigmatry.Entry.AspNetCore.Authorization
Enigmatry.Entry.SmartEnums*        ← SmartEnum + EF, Swagger, STJ, Verify helpers
Enigmatry.Entry.Csv / Email / BlobStorage / AzureSearch / GraphApi / HealthChecks / Scheduler / TemplatingEngine.*
```

`Enigmatry.Entry.Core` has no dependencies on the other modules and should stay that way.

## Key conventions

### Entities
- All domain entities inherit `Entity` (base with domain-event support) → `EntityWithTypedId<TId>` → `EntityWithGuidId` (auto-generates sequential GUIDs in the constructor).
- Domain events are `abstract record`s that inherit `DomainEvent` (which implements `INotification`).
- Events are raised via `AddDomainEvent()` inside the entity constructor or methods; the `PublishDomainEventsInterceptor` dispatches them via MediatR on `SaveChangesAsync`.

### CQRS
- Commands implement `ICommand` (no return) or `ICommand<TResponse>`.
- Queries implement `IQuery<TResponse>`.
- Both are MediatR `IRequest` wrappers. `IBaseCommand` carries `CommandTransactionBehavior` (default wraps the handler in a DB transaction via `CommandTransactionBehavior`).

### MediatR pipeline
Registered behaviors (in order): `ValidationBehavior` → `LoggingBehavior`. Validation runs FluentValidation validators; failures throw `ValidationException` before the handler is reached.

### EF DbContext
Extend `BaseDbContext`. Pass `EntitiesDbContextOptions` to the constructor; it controls which assembly is scanned for `IEntityTypeConfiguration` and entity registration.

### NuGet Central Package Management
All version numbers live in `Directory.Packages.props`. Never specify a `Version` attribute in a `.csproj` — only reference the package name. Transitive pinning is enabled (`CentralPackageTransitivePinningEnabled=true`).

### Build settings (Directory.Build.props)
- `TreatWarningsAsErrors=true` and `EnforceCodeStyleInBuild=true` are global — all warnings are errors.
- `LangVersion=14.0`, `Nullable=enable`, `ImplicitUsings=enable`.
- `MediatR` is pinned to an exact version (`[12.4.1]`) to prevent accidental upgrades.

### Test conventions
- Test classes are named `*Fixture` or `*Tests`.
- Every test class carries `[Category("unit")]`, `[Category("integration")]`, or `[Category("smoke")]`.
- Only `unit` and `smoke` categories run in CI; `integration` tests (e.g., Azure services, Testcontainers) are skipped.
- Assertions use **Shouldly** (`result.ShouldBe(...)`) or the **Verify** library for snapshot testing.
- Mocking uses **FakeItEasy** (`A.Fake<T>()`, `A.CallTo(...).MustHaveHappened(...)`).
- Test infrastructure helpers are placed in `Enigmatry.Entry.AspNetCore.Tests.Utilities` (shared across integration tests).

### SmartEnums
Use `Ardalis.SmartEnum` as the base. Companion packages add EF Core value converters (`SmartEnums.EntityFramework`), System.Text.Json converters (`SmartEnums.SystemTextJson`), and Swagger schema support (`SmartEnums.Swagger`).
