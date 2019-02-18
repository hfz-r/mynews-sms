using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Api.Models.ApiSettings.Clients
{
    public class ClientModel
    {
        public int Id { get; set; }

        [Display(Name = "Client name")]
        public string ClientName { get; set; }

        [Display(Name = "Callback url")]
        public string RedirectUrl { get; set; }

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
    }
}