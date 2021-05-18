using System;
using System.Linq;
using CachingFramework.Redis;
using CachingFramework.Redis.Contracts.Providers;
using CachingFramework.Redis.NewtonsoftJson;
using Microsoft.Extensions.Logging;
using Polly;

namespace Enigmatry.BuildingBlocks.CacheManager
{
    internal class RedisCacheManager : ICacheManager
    {
        private const string AllTag = "all";
        private readonly Lazy<RedisContext> _context;
        private readonly ILogger<RedisCacheManager> _logger;
        private readonly Policy _policy;

        public RedisCacheManager(string cacheConnectionString, ILogger<RedisCacheManager> logger, Policy retryPolicy)
        {
            _context = new Lazy<RedisContext>(() => new RedisContext(cacheConnectionString, new NewtonsoftJsonSerializer()));
            _logger = logger;
            _policy = retryPolicy;
        }

        private ICacheProvider Cache => _context.Value.Cache;

        public T Get<T>(string key)
        {
            _logger.LogDebug("Getting from cache, Type: {Type} by key: {Key}", typeof(T), key);

            var result = _policy
                .Execute(() => Cache.GetObject<T>(key));

            _logger.LogDebug(result != null ? "Cache hit" : "Cache miss");

            return result;
        }

        public void Remove(string key)
        {
            _logger.LogDebug("Removing from cache: {Key}", key);

            string[] tags = { AllTag };

            _policy.Execute(() =>
            {
                Cache.RemoveTagsFromKey(key, tags);
                Cache.Remove(key);
            });
        }

        public void Set<T>(string key, T value, TimeSpan timeout)
        {
            string[] tags = { AllTag };
            _policy.Execute(() =>
            {
                Cache.SetObject(key, value, tags.ToArray(), timeout);
            });
        }

        public void RemoveAllByPattern(string pattern)
        {
            _policy.Execute(() =>
            {
                _logger.LogInformation("Removing all: {KeyPattern}", pattern);
                var keys = Cache.GetKeysByPattern(pattern).ToArray();
                _logger.LogInformation("Number of keys to remove: {KeysCount}", keys.Length);

                foreach (var key in keys)
                {
                    string[] tags = { AllTag };
                    Cache.RemoveTagsFromKey(key, tags);
                }

                Cache.Remove(keys);
            });
        }
    }
}
