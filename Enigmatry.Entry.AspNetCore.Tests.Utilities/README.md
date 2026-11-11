# ASP.NET Core Tests Utilities

Helpers for integration tests of Entry-based ASP.NET Core APIs: a test host, a SQL Server test database with data reset between tests, and an in-memory EF Core database.

## Requirements

- NUnit 5. Diagnostic output goes to `TestContext.Out`.
- Docker, when `TestDatabase` has to start a SQL Server container (see below).
- An Autofac container in the application under test, for `ApiEnvironment<T>` and `InMemoryDbModule`.

## Installation

```bash
dotnet add package Enigmatry.Entry.AspNetCore.Tests.Utilities
```

## Test host

`ApiEnvironment<T>` is a `WebApplicationFactory<T>` that runs the API in the `Test` environment, loads `appsettings.Test.json`, and returns an `HttpClient` for `https://localhost` with a 30 second timeout. Override `OnContainerBuilderConfiguration` to replace Autofac registrations.

```csharp
public class TestEnvironment : ApiEnvironment<Program>
{
    public override void OnContainerBuilderConfiguration(HostBuilderContext context, ContainerBuilder builder)
    {
        base.OnContainerBuilderConfiguration(context, builder);
        builder.RegisterModule<InMemoryDbModule>();
    }
}

[Test]
public async Task GetsWeatherForecasts()
{
    await using var environment = new TestEnvironment();
    await TestRunner.Run(environment, async (_, client) =>
    {
        var response = await client.GetAsync(new Uri("weatherforecast", UriKind.Relative));
        response.EnsureSuccessStatusCode();
    });
}
```

`InMemoryDbModule` registers `DbContextOptions` for an EF Core in-memory database with a unique name. `UriExtensions.AppendParameters` adds query-string parameters to a `Uri`, and `ServiceScopeExtensions.Resolve<T>` resolves a required service from an `IServiceScope`.

## SQL Server test database

`TestDatabase.Create` reads the connection strings from the environment variables listed in `DatabaseInitializerOptions.ConnectionStringEnvironmentVariables` (default: `IntegrationTestsConnectionString`). When any of them is unset, it starts one shared SQL Server container through Testcontainers (`SqlContainerImage`) and uses that container's connection string for the unset ones. Disposing the last `TestDatabase` stops the container.

`Reset(dbContext)` prepares the database for a test:

- When the database doesn't exist or has pending migrations, it drops all objects and applies the migrations. Data is then reset only when `ResetDataEnabled` is set.
- Otherwise it deletes all data with Respawn, skipping `TablesToIgnore` and reseeding identity columns unless `ReseedIdentityColumns` is false. `BeforeDeleteCustomSqlQuery` and `AfterDeleteCustomSqlQuery` run around the delete.

```csharp
private static TestDatabase _database = null!;

[OneTimeSetUp]
public static async Task CreateDatabase() =>
    _database = await TestDatabase.Create(new DatabaseInitializerOptions { TablesToIgnore = ["__EFMigrationsHistory"] });

[SetUp]
public async Task ResetDatabase()
{
    await using var dbContext = CreateDbContext(_database.ConnectionStrings["IntegrationTestsConnectionString"]);
    await _database.Reset(dbContext);
}

[OneTimeTearDown]
public static async Task DisposeDatabase() => await _database.DisposeAsync();
```

Migrating needs at least one migration in the `DbContext`: EF Core throws when `MigrateAsync` finds none. `DatabaseHelpers.DropAllSql` holds the script that drops all database objects.
