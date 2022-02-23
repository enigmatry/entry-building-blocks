using Enigmatry.BuildingBlocks.Core.Helpers;

namespace Enigmatry.BuildingBlocks.AspNetCore.Tests.Database
{
    public static class DatabaseHelpers
    {
        public static string DropAllSql =>
            EmbeddedResource.ReadResourceContent(
                "Enigmatry.BuildingBlocks.AspNetCore.Tests.Database.DropAllSql.sql", typeof(DatabaseHelpers).Assembly);
    }
}
