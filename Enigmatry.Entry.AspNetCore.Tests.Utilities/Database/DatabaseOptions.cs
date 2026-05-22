namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

public sealed record DatabaseInitializerOptions
{
    public IEnumerable<string> TablesToIgnore { get; init; } = [];
    public string? BeforeDeleteCustomSqlQuery { get; init; }
    public string? AfterDeleteCustomSqlQuery { get; init; }
    public bool ReseedIdentityColumns { get; init; } = true;
}
