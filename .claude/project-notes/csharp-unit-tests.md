# Project notes — Entry building blocks

Project-specific companion to the shared `entry-blueprint:csharp-unit-tests` skill. The skill is the org standard; everything here is specific to this repo. Headings marked *Open question* are not verified yet.

## Test projects

Each tested package has a sibling `Enigmatry.Entry.<Module>.Tests` project. Tests use the `unit` and `integration` categories; CI runs `--filter "TestCategory=unit|TestCategory=smoke"`.

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

*Open question:* code books and builder locations are not inventoried yet.

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
