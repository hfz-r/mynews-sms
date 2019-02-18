using FluentValidation;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Web.Validators;

namespace StockManagementSystem.Validators.Users
{
    public class ForgotPasswordConfirmValidator : BaseValidator<ForgotPasswordConfirmViewModel>
    {
        public ForgotPasswordConfirmValidator(UserSettings userSettings)
        {
            RuleFor(x => x.NewPassword).IsPassword(userSettings);
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword)
                .WithMessage("The new password and confirmation password do not match.");
        }
    }
}