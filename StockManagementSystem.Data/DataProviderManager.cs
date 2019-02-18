using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Data
{
    public class DataProviderManager : IDataProviderManager
    {
        public IDataProvider DataProvider
        {
            get
            {
                var providerName = DataSettingsManager.LoadSettings()?.DataProvider;
                switch (providerName)
                {
                    case DataProviderType.SqlServer:
                        return new SqlServerDataProvider();

                    //TODO: other data providers

                    default:
                        throw new DefaultException($"Not supported data provider name: '{providerName}'");
                }
            }
        }
    }
}