using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

public sealed class TestDatabase
{
    public IReadOnlyDictionary<string, string> ConnectionStrings { get; }

    private static readonly Lock ContainerLock = new();
    private static MsSqlContainer? _container;
    private static bool _containerInitialized;
    private readonly DatabaseInitializerOptions _initializerOptions;

    public TestDatabase(DatabaseInitializerOptions initializerOptions)
    {
        _initializerOptions = initializerOptions;

        var resolved = new Dictionary<string, string>();
        var unresolved = new List<string>();

        foreach (var envVarName in initializerOptions.ConnectionStringEnvironmentVariables)
        {
            var value = Environment.GetEnvironmentVariable(envVarName);
            if (!string.IsNullOrEmpty(value))
            {
                resolved[envVarName] = value;
            }
            else
            {
                unresolved.Add(envVarName);
            }
        }

        if (unresolved.Count > 0)
        {
            try
            {
                InitializeContainer();
                initializerOptions.OnAfterContainerInitialized(_container!.GetConnectionString(), unresolved, resolved);
            }
            catch (Exception e)
            {
                WriteLine($"Failed to start docker container: {e.Message}");
                throw;
            }
        }

        ConnectionStrings = resolved;
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
            _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-CU4-ubuntu-24.04")
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

    private static void WriteLine(string value) => DatabaseInitializer.WriteLine(value);
}
