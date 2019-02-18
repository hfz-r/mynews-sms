namespace StockManagementSystem.Core.Data
{
    /// <summary>
    /// Represents a data provider manager
    /// </summary>
    public interface IDataProviderManager
    {
        IDataProvider DataProvider { get; }
    }
}