using Microsoft.EntityFrameworkCore;

namespace StockManagementSystem.Data.Mapping
{
    public partial interface IMappingConfiguration
    {
        void ApplyConfiguration(ModelBuilder modelBuilder);
    }
}
