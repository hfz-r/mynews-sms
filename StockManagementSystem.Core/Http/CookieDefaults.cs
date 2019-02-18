namespace StockManagementSystem.Core.Http
{
    public class CookieDefaults
    {
        public static string Prefix => ".myNsms";

        public static string UserCookie => ".User";

        public static string AntiforgeryCookie => ".Antiforgery";

        public static string SessionCookie => ".Session";

        public static string TempDataCookie => ".TempData";

        public static string AuthenticationCookie => ".Authentication";

        public static string ExternalAuthenticationCookie => ".ExternalAuthentication";
    }
}