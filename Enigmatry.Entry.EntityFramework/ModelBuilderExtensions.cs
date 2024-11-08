using Enigmatry.Entry.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Enigmatry.Entry.EntityFramework;

internal static class ModelBuilderExtensions
{
    internal static void RegisterEntities(this ModelBuilder modelBuilder, EntitiesDbContextOptions options)
    {
        var entityMethod =
            typeof(ModelBuilder).GetMethods().First(m => m is { Name: "Entity", IsGenericMethod: true });

        var entitiesAssembly = options.EntitiesAssembly;
        var types = entitiesAssembly.GetTypes();

        var entityTypes = types
            .Where(x => x.IsSubclassOf(typeof(Entity)) && !x.IsAbstract);

        foreach (var type in entityTypes)
        {
            if (options.EntityTypePredicate != null &&
                !options.EntityTypePredicate(type))
            {
                continue;
            }

            entityMethod.MakeGenericMethod(type).Invoke(modelBuilder, []);
        }
    }
}
