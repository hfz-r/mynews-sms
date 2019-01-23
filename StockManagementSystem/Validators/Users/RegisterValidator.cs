using FluentValidation;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Web.Validators;

namespace StockManagementSystem.Validators.Users
{
    public class RegisterValidator : BaseValidator<RegisterViewModel>
    {
        public RegisterValidator()
        {
            //username
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");

            //email
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Wrong email.");

            //password
            RuleFor(x => x.Password).IsPassword();

            //confirm password
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("The password and confirmation password do not match.");
        }
    }
}