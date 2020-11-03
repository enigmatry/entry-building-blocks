using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enigmatry.Blueprint.BuildingBlocks.CacheManager
{
    public static class RedisCacheStartupExtension
    {
        public static void AppAddRedisCacheManager(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICacheManager, RedisCacheManager>();
        }
    }
}
