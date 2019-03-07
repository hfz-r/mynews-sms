using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Stores
{
    public class UserStoreSearchModel : BaseSearchModel
    {
        /// <summary>
        /// This will represent by the BranchNo
        /// </summary>
        public int StoreId { get; set; }
    }
}