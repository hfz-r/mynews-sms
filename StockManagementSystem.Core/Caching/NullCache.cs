using System;
using System.Threading.Tasks;

namespace StockManagementSystem.Core.Caching
{
    /// <summary>
    /// Represents a null cache (caches nothing)
    /// </summary>
    public partial class NullCache : IStaticCacheManager
    {
        public virtual T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
            return default(T);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null)
        {
            return await Task.Run(() => default(T));
        }

        public virtual void Set(string key, object data, int cacheTime)
        {
        }

        public bool IsSet(string key)
        {
            return false;
        }

        public virtual void Remove(string key)
        {
        }

        public virtual void RemoveByPattern(string pattern)
        {
        }

        public virtual void Clear()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}