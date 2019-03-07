using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Core.Domain.Stores
{
    public class StoreUserAssignStores : BaseEntity
    {
        public int StoreUserAssignId { get; set; }

        public int UserId { get; set; }

        public virtual StoreUserAssign StoreUserAssigns { get; set; }

        public virtual User User { get; set; }
    }
}
