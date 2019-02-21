namespace StockManagementSystem.Core.Domain.Configuration
{
    public class Setting : BaseEntity
    {
        public Setting()
        {
        }

        public Setting(string name, string value, int tenantId = 0)
        {
            this.Name = name;
            this.Value = value;
            this.TenantId = tenantId;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public int TenantId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}