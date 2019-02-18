using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using StockManagementSystem.Validators.Users;
using StockManagementSystem.Web.Models;
using StockManagementSystem.Web.Mvc.ModelBinding;

namespace StockManagementSystem.Models.Account
{
    [Validator(typeof(LoginValidator))]
    public class LoginViewModel : BaseModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
