using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Core
{
    /// <summary>
    /// Store context
    /// </summary>
    public interface IStoreContext
    {
        Store CurrentStore { get; }

        int ActiveStoreScopeConfiguration { get; }
    }
}