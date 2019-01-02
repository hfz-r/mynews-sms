using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace StockManagementSystem.Core.Caching
{
    /// <summary>
    /// Represents a manager for caching during an HTTP request (short term caching)
    /// </summary>
    /// <inheritdoc />
    public partial class PerRequestCacheManager : ICacheManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PerRequestCacheManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets a key/value collection that can be used to share data within the scope of this request 
        /// </summary>
        protected virtual IDictionary<object, object> GetItems()
        {
            return _httpContextAccessor.HttpContext?.Items;
        }

        public virtual T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
            var items = GetItems();
            if (items == null)
                return acquire();

            //item already is in cache, so return it
            if (items[key] != null)
                return (T)items[key];

            //or create it using passed function
            var result = acquire();

            //and set in cache (if cache time is defined)
            if (result != null && (cacheTime ?? CacheConfig.CacheTime) > 0)
                items[key] = result;

            return result;
        }

        public void Set(string key, object data, int cacheTime)
        {
            var items = GetItems();
            if (items==null)
                return;

            if (data != null)
                items[key] = data;
        }

        public bool IsSet(string key)
        {
            var items = GetItems();

            return items?[key] != null;
        }

        public void Remove(string key)
        {
            var items = GetItems();

            items?.Remove(key);
        }

        public void RemoveByPattern(string pattern)
        {
            var items = GetItems();
            if (items == null)
                return;

            //get cache keys that matches pattern
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = items.Keys.Select(p => p.ToString()).Where(key => regex.IsMatch(key)).ToList();

            //remove matching values
            foreach (var key in matchesKeys)
            {
                items.Remove(key);
            }
        }

        public void Clear()
        {
            var items = GetItems();

            items?.Clear();
        }

        public void Dispose()
        {
        }
    }
}