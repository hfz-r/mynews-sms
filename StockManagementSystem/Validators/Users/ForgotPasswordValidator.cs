using FluentValidation;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Web.Validators;

namespace StockManagementSystem.Validators.Users
{
    public class ForgotPasswordValidator : BaseValidator<ForgotPasswordViewModel>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Enter your email");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Wrong email");
        }
    }
}