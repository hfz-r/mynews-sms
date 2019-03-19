using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using StockManagementSystem.Web.Mvc.Routing;

namespace StockManagementSystem.Web.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");

            //home page
            routeBuilder.MapRoute("HomePage", "",
                new { controller = "Home", action = "Index" });

            //login
            routeBuilder.MapRoute("Login", "login/",
                new { controller = "Account", action = "Login" });

            //register
            routeBuilder.MapRoute("Register", "register/",
                new { controller = "Account", action = "Register" });

            //logout
            routeBuilder.MapRoute("Logout", "logout/",
                new { controller = "Account", action = "Logout" });

            //forgot password
            routeBuilder.MapRoute("ForgotPassword", "forgotpassword",
                new { controller = "Account", action = "ForgotPassword" });

            //forgot password confirm
            routeBuilder.MapRoute("ForgotPasswordConfirm", "forgotpassword/confirm",
                new { controller = "Account", action = "ForgotPasswordConfirm" });

            //check username availability
            routeBuilder.MapRoute("CheckUsernameAvailability", "checkusernameavailability",
                new { controller = "Account", action = "CheckUsernameAvailability" });

            //user info
            routeBuilder.MapRoute("UserInfo", "account/info",
                new { controller = "Account", action = "Info" });

            //change password
            routeBuilder.MapRoute("UserChangePassword", "account/changepassword",
                new { controller = "Account", action = "ChangePassword" });

            //avatar
            routeBuilder.MapRoute("UserAvatar", "account/avatar",
                new { controller = "Account", action = "Avatar" });

            //install
            routeBuilder.MapRoute("Installation", "install",
                new { controller = "Install", action = "Index" });

            //error page
            routeBuilder.MapRoute("Error", "error",
                new { controller = "Common", action = "Error" });

            //page not found
            routeBuilder.MapRoute("PageNotFound", "page-not-found",
                new { controller = "Common", action = "PageNotFound" });

            //TODO: routes
        }

        public int Priority => 0;
    }
}