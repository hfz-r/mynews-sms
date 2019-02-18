using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Stores
{
    public class StoreUserAssign : Entity
    {
        public int StoreId { get; set; }

        public virtual Store Store { get; set; }

        public virtual ICollection<StoreUserAssignStores> StoreUserAssignStore { get; set; }

    }
}
