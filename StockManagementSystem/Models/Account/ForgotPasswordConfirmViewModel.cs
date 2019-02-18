using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using StockManagementSystem.Validators.Users;
using StockManagementSystem.Web.Models;
using StockManagementSystem.Web.Mvc.ModelBinding;

namespace StockManagementSystem.Models.Account
{
    [Validator(typeof(ForgotPasswordConfirmValidator))]
    public class ForgotPasswordConfirmViewModel : BaseModel
    {
        [DataType(DataType.Password)]
        [NoTrim]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [NoTrim]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmNewPassword { get; set; }

        public bool DisablePasswordChanging { get; set; }
        public string Result { get; set; }
    }
}