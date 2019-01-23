using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Models.System;
using StockManagementSystem.Services.Common;

namespace StockManagementSystem.ViewComponents
{
    [ViewComponent(Name = "PanelMasterComponent")]
    public class PanelMasterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
