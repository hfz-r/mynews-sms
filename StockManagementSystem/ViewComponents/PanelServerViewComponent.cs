using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Models.System;
using StockManagementSystem.Services.Common;

namespace StockManagementSystem.ViewComponents
{
    [ViewComponent(Name = "PanelServerComponent")]
    public class PanelServerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            ServerViewModel model = new ServerViewModel {AppVersion = AppInfo.GetVersion()};
            return View(model);
        }
    }
}
