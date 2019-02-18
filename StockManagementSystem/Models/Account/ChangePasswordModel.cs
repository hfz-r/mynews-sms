using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using StockManagementSystem.Validators.Users;
using StockManagementSystem.Web.Models;
using StockManagementSystem.Web.Mvc.ModelBinding;

namespace StockManagementSystem.Models.Account
{
    [Validator(typeof(ChangePasswordValidator))]
    public class ChangePasswordModel : BaseModel
    {
        [NoTrim]
        [DataType(DataType.Password)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }

        [NoTrim]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [NoTrim]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmNewPassword { get; set; }

        public string Result { get; set; }
    }
}
