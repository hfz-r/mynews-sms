using System.Collections.Generic;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Stores
{
    /// <summary>
    /// Represents a user model to add to the store
    /// </summary>
    public class AddUserToStoreModel : BaseModel
    {
        public AddUserToStoreModel()
        {
            SelectedUserIds = new List<int>();
        }

        public int StoreId { get; set; }

        public IList<int> SelectedUserIds { get; set; }
    }
}