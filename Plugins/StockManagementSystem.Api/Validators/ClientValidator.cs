using FluentValidation;
using IdentityServer4.EntityFramework.Entities;

namespace StockManagementSystem.Api.Validators
{
    public class ClientValidator : AbstractValidator<Client>
    {
        public ClientValidator()
        {
            RuleFor(x => x.ClientName).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.ClientId).NotEmpty().WithMessage("Client Id is required.");
            RuleFor(x => x.ClientSecrets).NotEmpty().WithMessage("Client Secret is required.");
            RuleFor(x => x.RedirectUris).NotEmpty().WithMessage("Callback Url is required");
        }
    }
}