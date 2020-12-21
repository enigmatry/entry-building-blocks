using System.Reflection;
using JetBrains.Annotations;

namespace Enigmatry.Blueprint.BuildingBlocks.EntityFramework
{
    [PublicAPI]
    public class EntitiesDbContextOptions
    {
        public Assembly ConfigurationAssembly { get; set; } = default!;

        public Assembly EntitiesAssembly { get; set; } = default!;
    }
}
