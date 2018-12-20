using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Users
{
    public partial class UserSearchModel : BaseSearchModel
    {
        public UserSearchModel()
        {
            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();
        }

        public IList<int> SelectedUserRoleIds { get; set; }

        public IList<SelectListItem> AvailableUserRoles { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string SearchEmail { get; set; }

        [Display(Name = "Username")]
        public string SearchUsername { get; set; }

        [Display(Name = "Name")]
        public string SearchName { get; set; }

        public string SearchBranch { get; set; }

        public string SearchDepartment { get; set; }
    }
}