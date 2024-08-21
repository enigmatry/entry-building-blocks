using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Enigmatry.Entry.SmartEnums.EntityFramework;

[PublicAPI]
public static class ModelConfigurationBuilderExtensions
{
    /// <summary>
    /// Configure SmartEnums for EntityFramework
    /// </summary>
    /// <param name="configurationBuilder"></param>
    public static void EntryConfigureSmartEnum(this ModelConfigurationBuilder configurationBuilder)
        => configurationBuilder.ConfigureSmartEnum();
}
