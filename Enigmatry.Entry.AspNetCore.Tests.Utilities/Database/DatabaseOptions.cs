namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

public sealed record DatabaseInitializerOptions
{
    public IEnumerable<string> TablesToIgnore { get; init; } = [];
    public string? BeforeDeleteCustomSqlQuery { get; init; }
    public string? AfterDeleteCustomSqlQuery { get; init; }
    public bool ReseedIdentityColumns { get; init; } = true;
    public IEnumerable<string> ConnectionStringEnvironmentVariables { get; init; } = ["IntegrationTestsConnectionString"];
    public Action<string, IReadOnlyList<string>, IDictionary<string, string>> OnAfterContainerInitialized { get; init; } =
        (containerConnectionString, unresolvedKeys, resolvedConnections) =>
        {
            foreach (var key in unresolvedKeys)
            {
                resolvedConnections[key] = containerConnectionString;
            }
            DatabaseInitializer.WriteLine($"Docker SQL connection string: {containerConnectionString}");
        };
}
