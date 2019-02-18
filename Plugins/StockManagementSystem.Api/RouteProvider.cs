using Microsoft.AspNetCore.Routing;
using StockManagementSystem.Web.Mvc.Routing;

namespace StockManagementSystem.Api
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
        }

        public int Priority => -1;
    }
}