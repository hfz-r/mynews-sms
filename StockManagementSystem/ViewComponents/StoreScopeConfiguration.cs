using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Factories;

namespace StockManagementSystem.ViewComponents
{
    /// <summary>
    /// Represents a view component that displays the store scope configuration
    /// </summary>
    public class StoreScopeConfigurationViewComponent : ViewComponent
    {
        private readonly ISettingModelFactory _settingModelFactory;

        public StoreScopeConfigurationViewComponent(ISettingModelFactory settingModelFactory)
        {
            this._settingModelFactory = settingModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //prepare model
            var model = await _settingModelFactory.PrepareStoreScopeConfigurationModel();

            if (model.Stores.Count < 2)
                return Content(string.Empty);

            return View(model);
        }
    }
}