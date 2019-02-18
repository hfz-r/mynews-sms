using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Factories;

namespace StockManagementSystem.ViewComponents
{
    public class SettingModeViewComponent : ViewComponent
    {
        private readonly ISettingModelFactory _settingModelFactory;

        public SettingModeViewComponent(ISettingModelFactory settingModelFactory)
        {
            this._settingModelFactory = settingModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string modeName = "settings-advanced-mode")
        {
            var model = await _settingModelFactory.PrepareSettingModeModel(modeName);

            return View(model);
        }
    }
}