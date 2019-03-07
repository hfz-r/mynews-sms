using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Stores
{
    public class AddUserToStoreSearchModel : BaseSearchModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string SearchEmail { get; set; }

        [Display(Name = "Username")]
        public string SearchUsername { get; set; }
    }
}