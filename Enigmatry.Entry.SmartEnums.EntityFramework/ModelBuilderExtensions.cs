using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Enigmatry.Entry.SmartEnums.EntityFramework;

[PublicAPI]
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Configure SmartEnums for EntityFramework
    /// </summary>
    /// <param name="modelBuilder"></param>
    public static void EntryConfigureSmartEnums(this ModelBuilder modelBuilder)
        => modelBuilder.ConfigureSmartEnum();
}
