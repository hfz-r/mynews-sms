using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using StockManagementSystem.Api.Validators;

namespace StockManagementSystem.Api.Models.ApiSettings.Clients
{
    [Validator(typeof(ClientValidator))]
    public class ClientModel
    {
        public ClientModel()
        {
            UrisSearchModel = new UrisSearchModel();
        }

        public int Id { get; set; }

        [Display(Name = "Client name")]
        public string ClientName { get; set; }

        [Display(Name = "Active")]
        public bool Enabled { get; set; }

        [Display(Name = "Client id")]
        public string ClientId { get; set; }

        [Display(Name = "Client secret")]
        public string ClientSecret { get; set; }

        [Display(Name = "Access token lifetime")]
        public int AccessTokenLifetime { get; set; }

        [Display(Name = "Refresh token lifetime")]
        public int RefreshTokenLifetime { get; set; }

        [Display(Name = "JavaScript client")]
        public bool JavaScriptClient { get; set; }

        public UrisSearchModel UrisSearchModel { get; set; }
    }
}