using System;

namespace SympliSeoTracker.Core.Interfaces
{
    public interface ICacheService
    {
        T Get<T>(string key);
        bool TryGetValue<T>(string key, out T value);
        void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null);
        void Remove(string key);
    }
}