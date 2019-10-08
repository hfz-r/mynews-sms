namespace StockManagementSystem.Core.Domain.Devices
{
    /// <summary>
    /// Represent Device-License mapping class
    /// </summary>
    public class DeviceLicense : BaseEntity
    {
        public int LicenseId { get; set; }

        public string SerialNo { get; set; }

        public virtual Device Device { get; set; }
    }
}