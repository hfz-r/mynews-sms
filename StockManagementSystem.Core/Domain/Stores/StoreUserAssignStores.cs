using StockManagementSystem.Core.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Stores
{
    public class StoreUserAssignStores : Entity
    {
        public int StoreUserAssignId { get; set; }

        public int UserId { get; set; }

        public virtual StoreUserAssign StoreUserAssigns { get; set; }

        public virtual User User { get; set; }
    }
}
