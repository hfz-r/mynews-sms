using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Stores
{
    /// <summary>
    /// Represents an entity which support store
    /// </summary>
    public interface IAppliedStoreSupported
    {
        IList<Store> AppliedStores { get; }
    }
}