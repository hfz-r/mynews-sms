using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace StockManagementSystem.Web.Events
{
    public class PageRenderingEvent
    {
        public PageRenderingEvent(IHtmlHelper helper, string overriddenRouteName = null)
        {
            Helper = helper;
            OverriddenRouteName = overriddenRouteName;
        }

        public IHtmlHelper Helper { get; private set; }

        public string OverriddenRouteName { get; private set; }

        public IEnumerable<string> GetRouteNames()
        {
            if (!string.IsNullOrEmpty(OverriddenRouteName))
            {
                return new List<string>() { OverriddenRouteName };
            }

            var matchedRoutes = Helper.ViewContext.RouteData.Routers.OfType<INamedRouter>();
            return matchedRoutes.Select(r => r.Name);
        }
    }
}