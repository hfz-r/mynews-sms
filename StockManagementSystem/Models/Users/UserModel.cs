using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;
using StockManagementSystem.Web.Mvc.ModelBinding;

namespace StockManagementSystem.Models.Users
{
    public class UserModel : BaseEntityModel, IAclSupportedModel
    {
        public UserModel()
        {
            SelectedRoleIds = new List<int>();
            AvailableRoles = new List<SelectListItem>();
            UserActivityLogSearchModel = new UserActivityLogSearchModel();
        }

        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        public string Password { get; set; }

        public string Name { get; set; }

        [Display(Name = "Admin comment")]
        public string AdminComment { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last activity")]
        public DateTime LastActivityDate { get; set; }

        [Display(Name = "IP Address")]
        public string LastIpAddress { get; set; }

        [Display(Name = "User roles")]
        public string UserRolesName { get; set; }

        [Display(Name = "Roles")]
        public IList<int> SelectedRoleIds { get; set; }

        public IList<SelectListItem> AvailableRoles { get; set; }

        public UserActivityLogSearchModel UserActivityLogSearchModel { get; set; }

        public SendEmailModel SendEmail { get; set; }
    }

    public partial class SendEmailModel : BaseModel
    {
        //TODO: wire with Azira`s module
    }
}