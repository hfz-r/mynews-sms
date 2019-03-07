using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Validators.Users;
using StockManagementSystem.Web.Models;
using StockManagementSystem.Web.Mvc.ModelBinding;

namespace StockManagementSystem.Models.Users
{
    [Validator(typeof(UserValidator))]
    public class UserModel : BaseEntityModel, IAclSupportedModel, IAppliedStoreSupportedModel
    {
        public UserModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
            UserActivityLogSearchModel = new UserActivityLogSearchModel();

            SelectedRoleIds = new List<int>();
            AvailableRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        public bool UsernamesEnabled { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Name")]
        public string FullName { get; set; }

        public bool DateOfBirthEnabled { get; set; }

        [UIHint("DateNullable")]
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }

        public bool PhoneEnabled { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Admin comment")]
        public string AdminComment { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        //time zone
        [Display(Name = "Time zone")]
        public string TimeZoneId { get; set; }

        public bool AllowUsersToSetTimeZone { get; set; }

        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //registration date
        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last activity")]
        public DateTime LastActivityDate { get; set; }

        //IP address
        [Display(Name = "IP Address")]
        public string LastIpAddress { get; set; }

        [Display(Name = "Last visited page")]
        public string LastVisitedPage { get; set; }

        [Display(Name = "User roles")]
        public string UserRolesName { get; set; }

        //acl (roles)
        [Display(Name = "Roles")]
        public IList<int> SelectedRoleIds { get; set; }
        public IList<SelectListItem> AvailableRoles { get; set; }

        //stores
        [Display(Name = "Stores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public UserActivityLogSearchModel UserActivityLogSearchModel { get; set; }

        public SendEmailModel SendEmail { get; set; }
    }

    public partial class SendEmailModel : BaseModel
    {
    }
}