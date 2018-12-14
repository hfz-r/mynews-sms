using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StockManagementSystem.Controllers
{
    public class DeviceController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("Device");
        }

        [HttpGet]
        public IActionResult RegisterDevice()
        {
            return View("RegisterDevice");
        }
    }
}