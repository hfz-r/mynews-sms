using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Factories;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Web.Mvc;

namespace StockManagementSystem.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleModelFactory _roleModelFactory;
        private readonly RoleManager<Role> _roleManager;

        public RoleController(
            IRoleModelFactory roleModelFactory,
            RoleManager<Role> roleManager,
            ILogger<RoleController> logger)
        {
            _roleModelFactory = roleModelFactory;
            _roleManager = roleManager;

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
                return AccessDeniedKendoGridJson();

            var model = await _roleModelFactory.PrepareRoleListModelAync(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> RoleAdd(RoleModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});

            var role = new Role();
            role = model.ToEntity(role);
            await _roleManager.CreateAsync(role);

            return new NullJsonResult();
        }
    }
}