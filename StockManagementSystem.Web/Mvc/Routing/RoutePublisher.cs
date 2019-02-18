using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Web.Mvc.Routing
{
    public class RoutePublisher : IRoutePublisher
    {
        protected readonly ITypeFinder TypeFinder;

        public RoutePublisher(ITypeFinder typeFinder)
        {
            this.TypeFinder = typeFinder;
        }

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //find route providers provided by other assemblies
            var routeProviders = TypeFinder.FindClassesOfType<IRouteProvider>();

            //create and sort instances of route providers
            var instances = routeProviders
                .Select(routeProvider => (IRouteProvider) Activator.CreateInstance(routeProvider))
                .OrderByDescending(routeProvider => routeProvider.Priority);

            //register all provided routes
            foreach (var routeProvider in instances)
                routeProvider.RegisterRoutes(routeBuilder);
        }
    }
}