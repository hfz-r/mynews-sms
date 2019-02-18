using System.Collections.Generic;
using StockManagementSystem.Models.Stores;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    /// <summary>
    /// Represents a store scope configuration model
    /// </summary>
    public class StoreScopeConfigurationModel : BaseModel
    {
        public StoreScopeConfigurationModel()
        {
            Stores = new List<StoreModel>();
        }

        public int StoreId { get; set; }

        public IList<StoreModel> Stores { get; set; }
    }
}