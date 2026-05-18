using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

public sealed class TestDatabase
{
    public string ConnectionString { get; }

    private static readonly Lock ContainerLock = new();
    private static MsSqlContainer? _container;
    private static bool _containerInitialized;
    private readonly DatabaseInitializerOptions _initializerOptions;

    public TestDatabase(DatabaseInitializerOptions initializerOptions)
    {
        _initializerOptions = initializerOptions;

        // To use a local sqlServer instance, Create an Environment variable using R# Test Runner, with name "IntegrationTestsConnectionString"
        // and value: "Server=.;Database={DatabaseName};Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False"
        // Environment variable with the IntegrationTestsConnectionString is set in .runsettings file
        var connectionString = Environment.GetEnvironmentVariable("IntegrationTestsConnectionString");

        if (!string.IsNullOrEmpty(connectionString))
        {
            ConnectionString = connectionString;
        }
        else
        {
            try
            {
                InitializeContainer();
                ConnectionString = _container!.GetConnectionString();
                WriteLine($"Docker SQL connection string: {ConnectionString}");
            }
            catch (Exception e)
            {
                WriteLine($"Failed to start docker container: {e.Message}");
                throw;
            }
        }
    }

    private static void InitializeContainer()
    {
        if (_containerInitialized)
        {
            return;
        }

        lock (ContainerLock)
        {
            if (_containerInitialized)
            {
                return;
            }

            // These cannot be changed (it is hardcoded in MsSqlBuilder and changing any of them breaks starting of the container
            // default database: master
            // default username: sa
            // default password: yourStrong(!)Password
            _container = new MsSqlBuilder()
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .Build();

            Task.Run(async () =>
            {
                await _container.StartAsync();
            }).GetAwaiter().GetResult();

            _containerInitialized = true;
        }
    }

    public Task ResetAsync(DbContext dbContext) => DatabaseInitializer.RecreateDatabaseAsync(dbContext, _initializerOptions);

    private static void WriteLine(string value) => TestContext.Out.WriteLine(value);
}
