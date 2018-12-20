using System;
using System.Collections.Generic;

namespace StockManagementSystem.Core.Builder
{
    public class BaseSingleton
    {
        static BaseSingleton()
        {
            AllSingletons = new Dictionary<Type, object>();
        }

        public static IDictionary<Type, object> AllSingletons { get; set; }
    }
}