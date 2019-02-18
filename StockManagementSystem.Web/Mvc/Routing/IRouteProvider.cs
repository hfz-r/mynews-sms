using Microsoft.AspNetCore.Routing;

namespace StockManagementSystem.Web.Mvc.Routing
{
    public interface IRouteProvider
    {
        void RegisterRoutes(IRouteBuilder routeBuilder);

        int Priority { get; }
    }
}