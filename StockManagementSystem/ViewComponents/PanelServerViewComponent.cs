﻿using System;
using System.Collections.Generic;
using StockManagementSystem.System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.ViewModels.System;

namespace StockManagementSystem.ViewComponents
{
    [ViewComponent(Name = "PanelServerComponent")]
    public class PanelServerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            ServerViewModel model = new ServerViewModel();
            model.AppVersion = AppInfo.GetVersion();
            return View(model);
        }
    }
}
