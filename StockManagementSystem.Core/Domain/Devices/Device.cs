﻿using StockManagementSystem.Core.Domain.Stores;
using System;
using StockManagementSystem.Core.Domain.Tenants;
using StockManagementSystem.Core.Domain.PushNotifications;
using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Devices
{
    public class Device : BaseEntity, IAppendTimestamps, ITenantMappingSupported, IStoreMappingSupported
    {
        private ICollection<DeviceLicense> _deviceLicenses;

        public string SerialNo { get; set; }

        public string ModelNo { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        //public DateTime? StartDate { get; set; }

        //public DateTime? EndDate { get; set; }

        public string TokenId { get; set; }

        public int StoreId { get; set; }

        public string Status { get; set; }

        public virtual Store Store { get; set; }

        //tenant mapping
        public bool LimitedToTenants { get; set; }

        //store mapping
        public bool LimitedToStores { get; set; }

        public virtual ICollection<PushNotificationDevice> PushNotificationDevices { get; set; }

        public virtual ICollection<DeviceLicense> DeviceLicenses
        {
            get => _deviceLicenses ?? (_deviceLicenses = new List<DeviceLicense>());
            set => _deviceLicenses = value;
        }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion

    }
}
