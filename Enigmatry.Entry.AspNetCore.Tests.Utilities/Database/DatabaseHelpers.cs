using Enigmatry.Entry.Core.Helpers;
using JetBrains.Annotations;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;

[UsedImplicitly]
public static class DatabaseHelpers
{
    public static string DropAllSql =>
        EmbeddedResource.ReadResourceContent(
            $"{typeof(DatabaseHelpers).Namespace}.DropAllSql.sql", typeof(DatabaseHelpers).Assembly);
}
