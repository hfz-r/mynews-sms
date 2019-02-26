﻿using StockManagementSystem.Core.Domain.Stores;
using System;
using StockManagementSystem.Core.Domain.Tenants;

namespace StockManagementSystem.Core.Domain.Devices
{
    public class Device : Entity, ITenantMappingSupported
    {
        public string SerialNo { get; set; }

        public string ModelNo { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TokenId { get; set; }

        public int StoreId { get; set; }

        public string Status { get; set; }

        public virtual Store Store { get; set; }

        public bool LimitedToTenants { get; set; }
    }
}
