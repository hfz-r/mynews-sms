using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using StockManagementSystem.Validators.Users;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Account
{
    [Validator(typeof(ForgotPasswordValidator))]
    public class ForgotPasswordViewModel : BaseModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Your email address")]
        public string Email { get; set; }

        public string Result { get; set; }
    }
}
