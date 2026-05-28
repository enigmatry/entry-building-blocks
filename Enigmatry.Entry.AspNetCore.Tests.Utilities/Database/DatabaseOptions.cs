namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

public sealed class DatabaseInitializerOptions
{
    public IReadOnlyList<string> TablesToIgnore { get; init; } = [];
    public string? BeforeDeleteCustomSqlQuery { get; init; }
    public string? AfterDeleteCustomSqlQuery { get; init; }
    public bool ReseedIdentityColumns { get; init; } = true;
    public bool ResetDataEnabled { get; init; }
    public IReadOnlyList<string> ConnectionStringEnvironmentVariables { get; init; } = ["IntegrationTestsConnectionString"];
    public string SqlContainerImage { get; init; } = "mcr.microsoft.com/mssql/server:2025-CU4-ubuntu-24.04";
    public Func<string, IReadOnlyList<string>, IDictionary<string, string>, Task> OnAfterContainerInitialized { get; init; } =
        (containerConnectionString, unresolvedKeys, resolvedConnections) =>
        {
            foreach (var key in unresolvedKeys)
            {
                resolvedConnections[key] = containerConnectionString;
            }
            return Task.CompletedTask;
        };
}
