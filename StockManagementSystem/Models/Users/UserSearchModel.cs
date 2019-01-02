using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Users
{
    public partial class UserSearchModel : BaseSearchModel, IAclSupportedModel
    {
        public UserSearchModel()
        {
            SelectedRoleIds = new List<int>();
            AvailableRoles = new List<SelectListItem>();
        }

        [Display(Name = "Roles")]
        public IList<int> SelectedRoleIds { get; set; }

        public IList<SelectListItem> AvailableRoles { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string SearchEmail { get; set; }

        [Display(Name = "Username")]
        public string SearchUsername { get; set; }

        [Display(Name = "Name")]
        public string SearchName { get; set; }

        [Display(Name = "IP address")]
        public string SearchIpAddress { get; set; }
    }
}