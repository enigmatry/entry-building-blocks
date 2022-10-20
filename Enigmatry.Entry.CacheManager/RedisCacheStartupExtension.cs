using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.CacheManager
{
    public static class RedisCacheStartupExtension
    {
        public static void AppAddRedisCacheManager(this IServiceCollection services) =>
            services.AddSingleton<ICacheManager, RedisCacheManager>();
    }
}
