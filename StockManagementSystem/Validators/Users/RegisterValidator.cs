using FluentValidation;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Web.Validators;

namespace StockManagementSystem.Validators.Users
{
    public class RegisterValidator : BaseValidator<RegisterViewModel>
    {
        public RegisterValidator(UserSettings userSettings)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Wrong email.");

            if (userSettings.UsernamesEnabled)
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            }

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");

            //password rule
            RuleFor(x => x.Password).IsPassword(userSettings);

            //confirm password
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("The password and confirmation password do not match.");

            //form fields
            if (userSettings.DateOfBirthEnabled && userSettings.DateOfBirthRequired)
            {
                //entered?
                RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
                {
                    var dateOfBirth = x.ParseDateOfBirth();
                    if (!dateOfBirth.HasValue)
                        return false;

                    return true;
                })
                .WithMessage("Date of birth is required.");
            }

            if (userSettings.PhoneRequired && userSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required.");
            }
        }
    }
}