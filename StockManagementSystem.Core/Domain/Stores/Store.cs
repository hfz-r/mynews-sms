using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StockManagementSystem.Core.Domain.Stores
{
    public class Store : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int P_BranchNo { get; set; }

        public string P_Name { get; set; }

        public string P_RecStatus { get; set; }

        public string P_CompID { get; set; }

        public string P_SellPriceLevel { get; set; }

        public string P_AreaCode { get; set; }

        public string P_Addr1 { get; set; }

        public string P_Addr2 { get; set; }

        public string P_Addr3 { get; set; }

        public string P_State { get; set; }

        public string P_City { get; set; }

        public string P_Country { get; set; }

        public string P_PostCode { get; set; }

        public string P_Brand { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public virtual ICollection<Device> Device { get; set; }

        public virtual ICollection<PushNotificationStore> PushNotificationStores { get; set; }

        public virtual ICollection<OrderLimitStore> OrderLimitStores { get; set; }

        public virtual ICollection<ShelfLocation> ShelfLocations { get; set; }
    }
}
