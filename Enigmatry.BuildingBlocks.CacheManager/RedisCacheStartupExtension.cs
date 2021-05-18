using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.BuildingBlocks.CacheManager
{
    public static class RedisCacheStartupExtension
    {
        public static void AppAddRedisCacheManager(this IServiceCollection services) =>
            services.AddSingleton<ICacheManager, RedisCacheManager>();
    }
}
