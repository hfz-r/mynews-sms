using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using StockManagementSystem.Validators.Users;
using StockManagementSystem.Web.Mvc.ModelBinding;

namespace StockManagementSystem.Models.Account
{
    [Validator(typeof(RegisterValidator))]
    public class RegisterViewModel
    {
        //[Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        //[Required]
        //[EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [NoTrim]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
