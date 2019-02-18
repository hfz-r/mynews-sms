using IdentityServer4.EntityFramework.Entities;
using StockManagementSystem.Api.Models.ApiSettings.Clients;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class ClientMappings
    {
        public static ClientModel ToApiModel(this Client client)
        {
            return client.MapTo<Client, ClientModel>();
        }
    }
}