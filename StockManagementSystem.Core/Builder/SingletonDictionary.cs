using System.Collections.Generic;

namespace StockManagementSystem.Core.Builder
{
    public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    {
        static SingletonDictionary()
        {
            Singleton<Dictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// The singleton instance for the specified type T. Only one instance (at the time) of this dictionary for each type of T.
        /// </summary>
        public new static IDictionary<TKey, TValue> Instance => Singleton<Dictionary<TKey, TValue>>.Instance;
    }
}