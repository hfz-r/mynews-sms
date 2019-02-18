using FluentValidation;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Web.Validators;

namespace StockManagementSystem.Validators.Users
{
    public class LoginValidator : BaseValidator<LoginViewModel>
    {
        public LoginValidator(UserSettings userSettings)
        {
            if (!userSettings.UsernamesEnabled)
            {
                //login by email
                RuleFor(x => x.Email).NotEmpty().WithMessage("Please enter your email");
                RuleFor(x => x.Email).EmailAddress().WithMessage("Wrong email");
            }
            else 
            {
                //login by username
                RuleFor(x => x.Username).NotEmpty().WithMessage("Please enter your username");
            }
        }
    }
}