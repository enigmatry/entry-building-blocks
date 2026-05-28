using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

public sealed class TestDatabase : IAsyncDisposable
{
    public IReadOnlyDictionary<string, string> ConnectionStrings { get; }

    private static readonly SemaphoreSlim ContainerLock = new(1, 1);
    private static MsSqlContainer? _container;
    private static bool _containerInitialized;
    private static int _containerRefCount;
    private readonly DatabaseInitializerOptions _initializerOptions;
    private readonly bool _usesContainer;

    private TestDatabase(DatabaseInitializerOptions initializerOptions, IReadOnlyDictionary<string, string> connectionStrings, bool usesContainer)
    {
        _initializerOptions = initializerOptions;
        ConnectionStrings = connectionStrings;
        _usesContainer = usesContainer;
    }

    public static async Task<TestDatabase> Create(DatabaseInitializerOptions initializerOptions)
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
            return new TestDatabase(initializerOptions, resolved, usesContainer: false);
        }

        try
        {
            await InitializeContainer(initializerOptions.SqlContainerImage);
            var containerConnectionString = _container!.GetConnectionString();
            await initializerOptions.OnAfterContainerInitialized(containerConnectionString, unresolved, resolved);
            WriteLine($"Docker SQL connection string: {containerConnectionString}");
        }
        catch (Exception e)
        {
            WriteLine($"Failed to start docker container: {e.Message}");
            throw;
        }

        Interlocked.Increment(ref _containerRefCount);
        return new TestDatabase(initializerOptions, resolved, usesContainer: true);
    }

    private static async Task InitializeContainer(string sqlContainerImage)
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

            _container = new MsSqlBuilder(sqlContainerImage)
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

    [PublicAPI]
    public Task Reset(DbContext dbContext) => DatabaseInitializer.EnsureDatabaseReady(dbContext, _initializerOptions);

    public async ValueTask DisposeAsync()
    {
        if (!_usesContainer)
        {
            return;
        }

        await ContainerLock.WaitAsync();
        try
        {
            _containerRefCount--;
            if (_containerRefCount == 0 && _container is not null)
            {
                await _container.DisposeAsync();
                _container = null;
                _containerInitialized = false;
            }
        }
        finally
        {
            ContainerLock.Release();
        }
    }

    private static void WriteLine(string value) => DatabaseInitializer.WriteLine(value);
}
