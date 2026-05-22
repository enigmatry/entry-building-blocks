using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

public sealed class TestDatabase
{
    public IReadOnlyDictionary<string, string> ConnectionStrings { get; }

    private static readonly SemaphoreSlim ContainerLock = new(1, 1);
    private static MsSqlContainer? _container;
    private static bool _containerInitialized;
    private readonly DatabaseInitializerOptions _initializerOptions;

    private TestDatabase(DatabaseInitializerOptions initializerOptions, IReadOnlyDictionary<string, string> connectionStrings)
    {
        _initializerOptions = initializerOptions;
        ConnectionStrings = connectionStrings;
    }

    public static async Task<TestDatabase> CreateAsync(DatabaseInitializerOptions initializerOptions)
    {
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

        if (unresolved.Count == 0)
        {
            return new TestDatabase(initializerOptions, resolved);
        }

        try
        {
            await InitializeContainerAsync();
            initializerOptions.OnAfterContainerInitialized(_container!.GetConnectionString(), unresolved, resolved);
        }
        catch (Exception e)
        {
            WriteLine($"Failed to start docker container: {e.Message}");
            throw;
        }

        return new TestDatabase(initializerOptions, resolved);
    }

    private static async Task InitializeContainerAsync()
    {
        if (_containerInitialized)
        {
            return;
        }

        await ContainerLock.WaitAsync();
        try
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

            await _container.StartAsync();
            _containerInitialized = true;
        }
        finally
        {
            ContainerLock.Release();
        }
    }

    public Task ResetAsync(DbContext dbContext) => DatabaseInitializer.RecreateDatabaseAsync(dbContext, _initializerOptions);

    private static void WriteLine(string value) => DatabaseInitializer.WriteLine(value);
}
