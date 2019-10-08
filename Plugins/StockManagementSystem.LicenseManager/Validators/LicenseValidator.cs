using FluentValidation;
using StockManagementSystem.LicenseManager.Models;

namespace StockManagementSystem.LicenseManager.Validators
{
    public class LicenseValidator : AbstractValidator<LicenseModel>
    {
        public LicenseValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("License to name is required.");

            RuleFor(x => x.Email).NotEmpty().WithMessage("License to email is required.");
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}