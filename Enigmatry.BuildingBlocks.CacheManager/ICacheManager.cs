using System;

namespace Enigmatry.BuildingBlocks.CacheManager
{
    public interface ICacheManager
    {
        T Get<T>(string key);

        void Set<T>(string key, T value, TimeSpan timeout);

        void Remove(string key);

        void RemoveAllByPattern(string pattern);
    }
}
