using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Factories;

namespace StockManagementSystem.ViewComponents
{
    /// <summary>
    /// Represents a view component that displays the tenant scope configuration
    /// </summary>
    public class TenantScopeConfigurationViewComponent : ViewComponent
    {
        private readonly ISettingModelFactory _settingModelFactory;

        public TenantScopeConfigurationViewComponent(ISettingModelFactory settingModelFactory)
        {
            _settingModelFactory = settingModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //prepare model
            var model = await _settingModelFactory.PrepareTenantScopeConfigurationModel();

            if (model.Tenants.Count < 2)
                return Content(string.Empty);

            return View(model);
        }
    }
}