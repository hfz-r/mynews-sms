using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleModelFactory _roleModelFactory;

        public RoleController(
            IRoleModelFactory roleModelFactory,
            ILogger<RoleController> logger)
        {
            _roleModelFactory = roleModelFactory;

            Logger = logger;
        }

        public ILogger Logger { get; }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return AccessDeniedView();

            var model = await _roleModelFactory.PrepareRoleSearchModelAsync(new RoleSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> List(RoleSearchModel searchModel)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Logger.LogWarning("Access denied to render Json for Role");
                return AccessDeniedKendoGridJson();
            }

            var model = await _roleModelFactory.PrepareRoleListModelAync(searchModel);

            return Json(model);
        }
    }
}