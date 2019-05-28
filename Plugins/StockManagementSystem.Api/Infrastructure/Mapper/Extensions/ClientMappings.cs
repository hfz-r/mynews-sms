using IdentityServer4.EntityFramework.Entities;
using StockManagementSystem.Api.Models.ApiSettings.Clients;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class ClientMappings
    {
        public static ClientModel ToModel(this Client client)
        {
            return client.MapTo<Client, ClientModel>();
        }

        //Redirect uri
        public static RedirectUrisModel ToModel(this ClientRedirectUri entity)
        {
            return entity.MapTo<ClientRedirectUri, RedirectUrisModel>();
        }

        //Post-logout uri
        public static PostLogoutUrisModel ToModel(this ClientPostLogoutRedirectUri entity)
        {
            return entity.MapTo<ClientPostLogoutRedirectUri, PostLogoutUrisModel>();
        }

        //Cors origins uri
        public static CorsOriginUrisModel ToModel(this ClientCorsOrigin entity)
        {
            return entity.MapTo<ClientCorsOrigin, CorsOriginUrisModel>();
        }
    }
}