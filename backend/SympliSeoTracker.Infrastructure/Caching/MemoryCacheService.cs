using Microsoft.Extensions.Caching.Memory;
using SympliSeoTracker.Domain.Interfaces;

namespace SympliSeoTracker.Infrastructure.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            
            if (absoluteExpiration.HasValue)
            {
                cacheEntryOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
            }
            else
            {
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            }
            
            _memoryCache.Set(key, value, cacheEntryOptions);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}