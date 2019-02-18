﻿using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View("Dashboard");
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult PanelServerComponent()
        {
            return ViewComponent("PanelServerComponent");
        }

        public IActionResult PanelTxnApprovalComponent()
        {
            return ViewComponent("PanelTxnApprovalComponent");
        }
    }
}
