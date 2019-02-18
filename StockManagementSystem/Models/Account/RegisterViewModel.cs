using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Validators.Users;
using StockManagementSystem.Web.Models;
using StockManagementSystem.Web.Mvc.ModelBinding;

namespace StockManagementSystem.Models.Account
{
    [Validator(typeof(RegisterValidator))]
    public class RegisterViewModel : BaseModel
    {
        public RegisterViewModel()
        {
            AvailableTimeZones = new List<SelectListItem>();    
        }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [Display(Name = "Username")]
        public string Username { get; set; }

        public bool CheckUsernameAvailabilityEnabled { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        #region Form fields & properties

        public bool GenderEnabled { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public bool DateOfBirthEnabled { get; set; }
        [Display(Name = "Date of birth")]
        public int? DateOfBirthDay { get; set; }
        [Display(Name = "Date of birth")]
        public int? DateOfBirthMonth { get; set; }
        [Display(Name = "Date of birth")]
        public int? DateOfBirthYear { get; set; }
        public bool DateOfBirthRequired { get; set; }
        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            return dateOfBirth;
        }

        public bool PhoneEnabled { get; set; }
        public bool PhoneRequired { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        //time zone
        [Display(Name = "Time zone")]
        public string TimeZoneId { get; set; }
        public bool AllowUsersToSetTimeZone { get; set; }
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        #endregion
    }
}
