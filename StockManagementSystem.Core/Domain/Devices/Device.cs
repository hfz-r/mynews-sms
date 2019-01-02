using StockManagementSystem.Core.Domain.Stores;
using System;

namespace StockManagementSystem.Core.Domain.Devices
{
    public class Device : Entity
    {
        public string SerialNo { get; set; }

        public string ModelNo { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TokenId { get; set; }

        public int StoreId { get; set; }

        public string Status { get; set; }

        public virtual Store Store { get; set; }
    }
}
