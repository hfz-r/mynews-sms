using System;
using System.ComponentModel.DataAnnotations.Schema;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Services.Stores
{
    /// <summary>
    /// Store (for caching)
    /// </summary>
    [Serializable]
    [NotMapped]
    public class StoreForCaching : Store, IEntityForCaching
    {
        public StoreForCaching()
        {
        }

        public StoreForCaching(Store s)
        {
            P_BranchNo = s.P_BranchNo;
            P_Name = s.P_Name;
            P_RecStatus = s.P_RecStatus;
            P_CompID = s.P_CompID;
            P_SellPriceLevel = s.P_SellPriceLevel;
            P_AreaCode = s.P_AreaCode;
            P_Addr1 = s.P_Addr1;
            P_Addr2 = s.P_Addr2;
            P_Addr3 = s.P_Addr3;
            P_State = s.P_State;
            P_City = s.P_City;
            P_Country = s.P_Country;
            P_PostCode = s.P_PostCode;
            P_Brand = s.P_Brand;
            Url = s.Url;
            SslEnabled = s.SslEnabled;
            Hosts = s.Hosts;
        }
    }
}