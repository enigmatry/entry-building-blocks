using System;
using Microsoft.Extensions.Caching.Memory;

namespace Enigmatry.Blueprint.BuildingBlocks.CacheManager
{
    internal class MemoryCacheCacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheCacheManager(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void RemoveAllByPattern(string pattern)
        {
            // this one is not supported by cache
        }

        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out object value))
            {
                return (T)value;
            }

            return default(T)!;
        }

        public void Set<T>(string key, T value, TimeSpan timeout)
        {
            _cache.Set(key, value, timeout);
        }

        public void AddItemToSortedSet(string setId, object value, double score)
        {
            // this one is not supported by cache
        }

        public void AddItemToList(string listId, object value)
        {
            // this one is not supported by cache
        }
    }
}
