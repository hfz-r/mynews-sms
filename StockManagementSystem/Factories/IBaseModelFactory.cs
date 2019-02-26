using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockManagementSystem.Factories
{
    public interface IBaseModelFactory
    {
        Task PrepareActivityLogTypes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        Task PrepareLoadPluginModes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        Task PreparePluginGroups(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        Task PrepareRoles(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        Task PrepareStores(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        Task PrepareTimeZones(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        Task PrepareBranches(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
    }
}