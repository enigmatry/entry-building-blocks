using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NUnit.Framework;
using Respawn;
using Respawn.Graph;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

internal static class DatabaseInitializer
{
    public static async Task EnsureDatabaseReady(DbContext dbContext, DatabaseInitializerOptions options)
    {
        if (await HasSchemaChanges(dbContext))
        {
            await DropAndMigrate(dbContext);
            if (options.ResetDataEnabled)
            {
                await ResetData(dbContext, options);
            }
        }
        else
        {
            await ResetData(dbContext, options);
        }
    }

    private static async Task<bool> HasSchemaChanges(DbContext dbContext)
    {
        try
        {
            // CanConnectAsync throws SqlException if the server is unreachable; returns false if the database does not exist
            var dbDoesNotExist = !await dbContext.Database.CanConnectAsync();
            return dbDoesNotExist || (await dbContext.Database.GetPendingMigrationsAsync()).Any();
        }
        catch (SqlException ex)
        {
            WriteLine("Error connecting to SqlServer:");
            WriteLine(ex.ToString());
            throw;
        }
    }

    private static async Task DropAndMigrate(DbContext dbContext)
    {
        await DropAllDbObjects(dbContext.Database);
        await dbContext.Database.MigrateAsync();
    }

    private static async Task ResetData(DbContext dbContext, DatabaseInitializerOptions options)
    {
        await RunCustomQuery(dbContext, options.BeforeDeleteCustomSqlQuery);
        await DeleteData(dbContext, options);
        await RunCustomQuery(dbContext, options.AfterDeleteCustomSqlQuery);
    }

    private static async Task DeleteData(DbContext dbContext, DatabaseInitializerOptions options)
    {
        var connectionString = dbContext.Database.GetConnectionString() ?? string.Empty;

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var respawner = await Respawner.CreateAsync(connection,
            new RespawnerOptions
            {
                TablesToIgnore = options.TablesToIgnore.Select(name => new Table(name)).ToArray(),
                WithReseed = options.ReseedIdentityColumns
            });

        await respawner.ResetAsync(connection);
    }

    private static async Task RunCustomQuery(DbContext dbContext, string? customSqlQuery)
    {
        if (!string.IsNullOrEmpty(customSqlQuery))
        {
            await dbContext.Database.ExecuteSqlRawAsync(customSqlQuery);
        }
    }

    private static async Task DropAllDbObjects(DatabaseFacade database)
    {
        try
        {
            var dropAllSql = DatabaseHelpers.DropAllSql;
            foreach (var statement in dropAllSql.SplitStatements())
            {
                await database.ExecuteSqlRawAsync(statement);
            }
        }
        catch (SqlException ex)
        {
            const int cannotOpenDatabaseErrorNumber = 4060;
            if (ex.Number == cannotOpenDatabaseErrorNumber)
            {
                WriteLine("Error while trying to drop all objects from database. Maybe database does not exist.");
                WriteLine("Continuing...");
                WriteLine(ex.ToString());
            }
            else
            {
                throw;
            }
        }
    }

    internal static void WriteLine(string value) => TestContext.Out.WriteLine(value);
}
