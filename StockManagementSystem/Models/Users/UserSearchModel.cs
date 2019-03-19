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

        public bool UsernamesEnabled { get; set; }

        [Display(Name = "First name")]
        public string SearchFirstName { get; set; }

        [Display(Name = "Last name")]
        public string SearchLastName { get; set; }

        [Display(Name = "Date of birth")]
        public string SearchDayOfBirth { get; set; }

        [Display(Name = "Date of birth")]
        public string SearchMonthOfBirth { get; set; }

        public bool DateOfBirthEnabled { get; set; }

        [Display(Name = "Phone")]
        public string SearchPhone { get; set; }

        public bool PhoneEnabled { get; set; }

        [Display(Name = "IP address")]
        public string SearchIpAddress { get; set; }

        public bool AvatarEnabled { get; internal set; }
    }
}