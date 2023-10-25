using System;
using Microsoft.Extensions.Caching.Memory;

namespace Enigmatry.Entry.CacheManager
{
    public class MemoryCacheCacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheCacheManager(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Remove(string key) => _cache.Remove(key);

        public void RemoveAllByPattern(string pattern) => throw new NotSupportedException("Not supported by memory cache.");

        public T? Get<T>(string key) => _cache.TryGetValue(key, out var value) ? (T?)value : default;

        public void Set<T>(string key, T value, TimeSpan timeout) => _cache.Set(key, value, timeout);

        public void AddItemToSortedSet(string setId, object value, double score) => throw new NotSupportedException("Not supported by memory cache.");

        public void AddItemToList(string listId, object value) => throw new NotSupportedException("Not supported by memory cache.");
    }
}
