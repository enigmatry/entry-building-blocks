# Project notes — Entry building blocks

Project-specific companion to the shared `entry-blueprint:csharp-unit-tests` skill. The skill is the org standard; everything here is specific to this repo. Headings marked *Open question* are not verified yet.

## Test projects

Each tested package has a sibling `Enigmatry.Entry.<Module>.Tests` project. Tests use the `unit` and `integration` categories. The build pipeline runs `--filter "TestCategory=unit|TestCategory=smoke"`; the nightly code-analysis pipeline runs every test project, integration included. The `AzureSearch.Tests.Searching.*` fixtures are `[Explicit]` (BP-815) and only run on demand.

| Project | Categories | Integration profile |
|---|---|---|
| `AspNetCore.Tests` | `unit`, `integration` | in-process host (`WebApplicationFactory` over `AspNetCore.Tests.SampleApp`), no database |
| `AspNetCore.Tests.NewtonsoftJson.Tests`, `AspNetCore.Tests.SystemTextJson.Tests` | `integration` | same SampleApp host, per serializer |
| `AzureSearch.Tests` | `unit`, `integration` | real Azure AI Search service (see Test databases) |
| `EntityFramework.Tests` | `unit` | EF in-memory |
| all other `*.Tests` projects | `unit` | — |

## Integration test harness

`Enigmatry.Entry.AspNetCore.Tests` holds the repo's shared test infrastructure (`SampleAppFixtureBase`, `WeatherForecastControllerFixtureBase`, `HttpContextBuilder`, `ExceptionContextBuilder`, test impersonation); the two `*.Json.Tests` projects reference it. `Enigmatry.Entry.AspNetCore.Tests.Utilities` is a package shipped to consumers, for their own integration tests.

## Test databases

- The integration tests run in-process or against Azure AI Search. SQL Server through Testcontainers lives in the shipped `AspNetCore.Tests.Utilities` package.
- `AzureSearch.Tests` needs `AzureSearch:SearchServiceEndPoint` and `AzureSearch:ApiKey` through user secrets or the `AzureSearch__*` env vars. The shared test service holds at most 15 indexes; past that its tests fail with HTTP 429.

## Test data, builders and code books

- **Code-book shape:** C# 14 `extension(Type)` static members, one `internal static class <Type>CodeBook` per extended type, entries returning finished objects (`GraphUser.Some`). Same-named members from two `extension` blocks in one class collide, hence one class per type. Existing books: `GraphUserCodeBook`, `ObjectIdentityCodeBook`, `PasswordProfileCodeBook` in `Enigmatry.Entry.GraphApi.Tests`.
- **Builders** live next to the fixtures that use them, in the test project: `FakeGraphClientBuilder` (GraphApi), `TestDocumentBuilder` and `ServiceCollectionBuilder` (AzureSearch), `SchedulingConfigurationBuilder` (Scheduler), `HttpContextBuilder` and `ExceptionContextBuilder` (AspNetCore), `TestServiceProviderBuilder` (MediatR). A test helper that needs configuration gets a builder, not public mutable setters; a helper owning disposable resources is `IDisposable` and is disposed in `[TearDown]`.

## Verify

Snapshots (`*.verified.txt`) sit next to their fixtures in AzureSearch, GraphApi, MediatR, Scheduler, BlobStorage and AspNetCore.Tests. Verify runs with its default settings. `.gitattributes` forces `*.verified.*` to LF/UTF-8; `*.received.*` is gitignored. The AspNetCore snapshot sits next to `WeatherForecastControllerFixtureBase` and is shared by the derived fixtures in both `*.Json.Tests` projects.

## Time seams

`Enigmatry.Entry.Core.ITimeProvider` (`UtcNow`, `FixedUtcNow`), implemented by `Enigmatry.Entry.Infrastructure.TimeProvider`.

## Known legacy

- Fixtures named `*Tests` (e.g. `LiquidTemplatingEngineTests`, `PublishDomainEventsInterceptorTests`) and `public` fixtures.
- Test bodies without blank lines between arrange, act and assert.

Convert on touch.

## Other gotchas

- `TreatWarningsAsErrors` and the analyzer severities in `.editorconfig` apply to test projects too.
- `LiquidTemplatingEngineTests` renders with culture `nl-NL` and time zone `Europe/Amsterdam`.
- The development machine runs culture `en-NL`; check culture before calling a date or number failure a regression.
