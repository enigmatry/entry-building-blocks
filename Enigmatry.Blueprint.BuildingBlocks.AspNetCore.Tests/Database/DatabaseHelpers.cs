// unset

namespace Enigmatry.Blueprint.BuildingBlocks.AspNetCore.Tests.Database
{
    public static class DatabaseHelpers
    {
        public static string DropAllSql =>
            EmbeddedResource.ReadResourceContent(
                "Enigmatry.Blueprint.BuildingBlocks.AspNetCore.Tests.Database.DropAllSql.sql");
    }
}
