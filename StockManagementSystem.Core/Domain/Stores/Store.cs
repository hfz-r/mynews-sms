using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagementSystem.Core.Domain.Stores
{
    public class Store : BaseEntity, IAppendTimestamps
    {
        private ICollection<UserStore> _userStores;

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

        public bool Active { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public virtual ICollection<Device> Device { get; set; }

        public virtual ICollection<PushNotificationStore> PushNotificationStores { get; set; }

        public virtual ICollection<OrderLimitStore> OrderLimitStores { get; set; }

        public virtual ICollection<ReplenishmentStore> ReplenishmentStores { get; set; }

        public virtual ICollection<ShelfLocation> ShelfLocations { get; set; }

        public virtual ICollection<StoreGroupingStores> StoreGroupingStore { get; set; }

        public virtual ICollection<StoreUserAssign> StoreUserAssigns { get; set; }

        public virtual ICollection<UserStore> UserStores
        {
            get => _userStores ?? (_userStores = new List<UserStore>());
            protected set => _userStores = value;
        }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
