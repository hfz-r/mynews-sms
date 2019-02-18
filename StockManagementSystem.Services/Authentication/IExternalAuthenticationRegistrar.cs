using Microsoft.AspNetCore.Authentication;

namespace StockManagementSystem.Services.Authentication
{
    /// <summary>
    /// Interface to register (configure) an external authentication service
    /// </summary>
    public interface IExternalAuthenticationRegistrar
    {
        void Configure(AuthenticationBuilder builder);
    }
}