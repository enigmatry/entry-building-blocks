using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NUnit.Framework;
using Respawn;
using Respawn.Graph;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

internal static class DatabaseInitializer
{
    public static async Task RecreateDatabaseAsync(DbContext dbContext, DatabaseInitializerOptions options)
    {
        if (HasSchemaChanges(dbContext))
        {
            RecreateDatabase(dbContext);
        }
        else
        {
            await ResetDataAsync(dbContext, options);
        }
    }

    private static bool HasSchemaChanges(DbContext dbContext)
    {
        try
        {
            var dbDoesNotExist = !dbContext.Database.CanConnect(); // this will throw SqlException if connection to server can not be made, and true / false depending on if db exists
            return dbDoesNotExist || dbContext.Database.GetPendingMigrations().Any();
        }
        catch (SqlException ex)
        {
            WriteLine("Error connecting to SqlServer:");
            WriteLine(ex.ToString());
            throw;
        }
    }

    private static void RecreateDatabase(DbContext dbContext)
    {
        DropAllDbObjects(dbContext.Database);
        dbContext.Database.Migrate();
    }

    private static async Task ResetDataAsync(DbContext dbContext, DatabaseInitializerOptions options)
    {
        RunCustomQuery(dbContext, options.BeforeDeleteCustomSqlQuery);
        await DeleteDataAsync(dbContext, options);
        RunCustomQuery(dbContext, options.AfterDeleteCustomSqlQuery);
    }

    private static async Task DeleteDataAsync(DbContext dbContext, DatabaseInitializerOptions options)
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

    private static void RunCustomQuery(DbContext dbContext, string? customSqlQuery)
    {
        if (!string.IsNullOrEmpty(customSqlQuery))
        {
            dbContext.Database.ExecuteSqlRaw(customSqlQuery);
        }
    }

    private static void DropAllDbObjects(DatabaseFacade database)
    {
        try
        {
            var dropAllSql = DatabaseHelpers.DropAllSql;
            foreach (var statement in dropAllSql.SplitStatements())
            {
                database.ExecuteSqlRaw(statement);
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
