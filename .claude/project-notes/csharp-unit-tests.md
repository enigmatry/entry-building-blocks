# Project notes — Entry building blocks

Project-specific companion to the shared `entry-blueprint:csharp-unit-tests` skill. The skill is the org standard; everything here is specific to this repo. Headings marked *Open question* are not verified yet.

## Test projects

One `Enigmatry.Entry.<Module>.Tests` project per package. CI runs `--filter "TestCategory=unit|TestCategory=smoke"`; no test uses `smoke`.

| Project | Categories | Integration profile |
|---|---|---|
| `AspNetCore.Tests` | `unit`, `integration` | in-process host (`WebApplicationFactory` over `AspNetCore.Tests.SampleApp`), no database |
| `AspNetCore.Tests.NewtonsoftJson.Tests`, `AspNetCore.Tests.SystemTextJson.Tests` | `integration` | same SampleApp host, per serializer |
| `AzureSearch.Tests` | `unit`, `integration` | real Azure AI Search service (see Test databases) |
| `EntityFramework.Tests` | `unit` | EF in-memory |
| all other `*.Tests` projects | `unit` | — |

`Enigmatry.Entry.Randomness` has no test project.

## Integration test harness

`Enigmatry.Entry.AspNetCore.Tests` holds the repo's shared test infrastructure (`SampleAppFixtureBase`, `WeatherForecastControllerFixtureBase`, `HttpContextBuilder`, `ExceptionContextBuilder`, test impersonation); the two `*.Json.Tests` projects reference it. `Enigmatry.Entry.AspNetCore.Tests.Utilities` is a package shipped to consumers; no test project here references it.

## Test databases

- No test in this repo uses SQL Server or Testcontainers.
- `AzureSearch.Tests` needs `AzureSearch:SearchServiceEndPoint` and `AzureSearch:ApiKey` through user secrets or the `AzureSearch__*` env vars. The shared test service holds at most 15 indexes; past that its tests fail with HTTP 429.

## Test data, builders and code books

*Open question:* no code books found; builder locations not inventoried.

## Verify

Snapshots (`*.verified.txt`) sit next to their fixtures in AzureSearch, GraphApi, MediatR, Scheduler, BlobStorage and AspNetCore.Tests. There is no `[ModuleInitializer]` or shared `VerifySettings`. `.gitattributes` forces `*.verified.*` to LF/UTF-8; `*.received.*` is gitignored. The AspNetCore snapshot sits next to `WeatherForecastControllerFixtureBase` and is shared by the derived fixtures in both `*.Json.Tests` projects.

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
