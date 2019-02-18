using Microsoft.AspNetCore.Mvc;

namespace StockManagementSystem.Controllers
{
    public class KeepAliveController : Controller
    {
        public virtual IActionResult Index()
        {
            return Content("I am alive!");
        }
    }
}