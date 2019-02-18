using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using StockManagementSystem.Api.Services;

namespace StockManagementSystem.Api.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IClientService _clientService;

        public ProfileService(IClientService clientService)
        {
            _clientService = clientService;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");

            if (int.TryParse(sub?.Value, out _))
            {
                // TODO: do we need claims??
            }

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}