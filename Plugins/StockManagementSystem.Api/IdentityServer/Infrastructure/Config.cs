using System.Collections.Generic;
using IdentityServer4.Models;

namespace StockManagementSystem.Api.IdentityServer.Infrastructure
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResource()
        {
            return new List<ApiResource>
            {
                new ApiResource("sms_api", "myNEWS SMS Web API")
            };
        }
    }
}