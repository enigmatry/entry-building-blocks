using Enigmatry.Entry.Core.Helpers;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

public static class DatabaseHelpers
{
    public static string DropAllSql =>
        EmbeddedResource.ReadResourceContent(
            "Enigmatry.Entry.AspNetCore.Tests.Database.DropAllSql.sql", typeof(DatabaseHelpers).Assembly);
}
