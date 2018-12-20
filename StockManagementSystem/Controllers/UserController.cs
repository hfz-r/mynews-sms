using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        public UserController(
            ILogger<UserController> logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; }

        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> Index()
        //{
        //    if (!User.Identity.IsAuthenticated)
        //        return AccessDeniedView();

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> UserList()
        //{

        //    return Json(model);
        //}
    }
}