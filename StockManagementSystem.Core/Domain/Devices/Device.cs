namespace StockManagementSystem.Core.Domain.Devices
{
    public class Device : Entity
    {
        public string SerialNo { get; set; }

        public string ModelNo { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public string TokenId { get; set; }

        public string StoreId { get; set; }

        public string Status { get; set; }
    }
}
