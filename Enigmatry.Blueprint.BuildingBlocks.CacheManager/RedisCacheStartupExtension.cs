using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Blueprint.BuildingBlocks.CacheManager
{
    public static class RedisCacheStartupExtension
    {
        public static void AppAddRedisCacheManager(this IServiceCollection services) =>
            services.AddSingleton<ICacheManager, RedisCacheManager>();
    }
}
