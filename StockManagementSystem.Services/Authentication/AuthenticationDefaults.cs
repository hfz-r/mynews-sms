using Microsoft.AspNetCore.Http;

namespace StockManagementSystem.Services.Authentication
{
    /// <summary>
    /// Represents default values related to authentication services
    /// </summary>
    public static class AuthenticationDefaults
    {
        public static string AuthenticationScheme => "Authentication";

        public static string ExternalAuthenticationScheme => "ExternalAuthentication";

        public static string ClaimsIssuer => "smsAdmin";

        // or /Account/Login
        public static PathString LoginPath => new PathString("/login");

        public static PathString LogoutPath => new PathString("/logout");

        // or /Account/AccessDenied
        public static PathString AccessDeniedPath => new PathString("/page-not-found");

        public static string ReturnUrlParameter => string.Empty;

        public static string ExternalAuthenticationErrorsSessionKey => "externalauth.errors";
    }
}