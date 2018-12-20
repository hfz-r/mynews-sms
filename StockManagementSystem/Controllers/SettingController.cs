using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StockManagementSystem.Controllers
{
    public class SettingController : Controller
    {
        [HttpGet]
        public IActionResult Order()
        {
            return View("Order");
        }

        [HttpGet]
        public IActionResult Approval()
        {
            return View("Approval");
        }

        [HttpGet]
        public IActionResult AddOrder()
        {
            return View("AddOrder");
        }

        [HttpGet]
        public IActionResult Location()
        {
            return View("Location");
        }
    }
}