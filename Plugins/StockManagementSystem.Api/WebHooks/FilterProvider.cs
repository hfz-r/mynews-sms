using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebHooks;
using StockManagementSystem.Api.Constants;

namespace StockManagementSystem.Api.WebHooks
{
    public class FilterProvider : IWebHookFilterProvider
    {
        private readonly Collection<WebHookFilter> filters = new Collection<WebHookFilter>
        {
            new WebHookFilter {Name = WebHookNames.UsersCreated, Description = "A user has been created."},
            new WebHookFilter {Name = WebHookNames.UsersUpdated, Description = "A user has been updated."},
            new WebHookFilter {Name = WebHookNames.UsersDeleted, Description = "A user has been deleted."},

            new WebHookFilter {Name = WebHookNames.DevicesCreated, Description = "A device has been created."},
            new WebHookFilter {Name = WebHookNames.DevicesUpdated, Description = "A device has been updated."},
            new WebHookFilter {Name = WebHookNames.DevicesDeleted, Description = "A device has been deleted."},

            new WebHookFilter {Name = WebHookNames.ItemsCreated, Description = "An item has been created."},
            new WebHookFilter {Name = WebHookNames.ItemsUpdated, Description = "An item has been updated."},
            new WebHookFilter {Name = WebHookNames.ItemsDeleted, Description = "An item has been deleted."},
        };

        public Task<Collection<WebHookFilter>> GetFiltersAsync()
        {
            return Task.FromResult(filters);
        }
    }
}