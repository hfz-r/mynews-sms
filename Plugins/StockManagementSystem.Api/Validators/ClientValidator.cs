using FluentValidation;
using StockManagementSystem.Api.Models.ApiSettings.Clients;

namespace StockManagementSystem.Api.Validators
{
    public class ClientValidator : AbstractValidator<ClientModel>
    {
        public ClientValidator()
        {
            RuleFor(x => x.ClientName).NotEmpty().WithMessage("Client name is required.");
            RuleFor(x => x.ClientId).NotEmpty().WithMessage("Client Id is required.");
            RuleFor(x => x.ClientSecret).NotEmpty().WithMessage("Client secret is required.");
            //remove this to RedirectUrisModel
            //RuleFor(x => x.RedirectUris).NotEmpty().WithMessage("Callback Url is required");
        }
    }
}