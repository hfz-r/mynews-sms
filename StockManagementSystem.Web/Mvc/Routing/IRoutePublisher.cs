using Microsoft.AspNetCore.Routing;

namespace StockManagementSystem.Web.Mvc.Routing
{
    public interface IRoutePublisher
    {
        void RegisterRoutes(IRouteBuilder routeBuilder);
    }
}