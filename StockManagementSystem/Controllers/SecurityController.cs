using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class SecurityController : BaseController
    {
        private readonly ILogger _logger;
        private readonly UserManager<User> _userManager;

        public SecurityController(ILogger<SecurityController> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> AccessDenied(string pageUrl)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !User.Identity.IsAuthenticated)
            {
                _logger.LogWarning($"Access denied to anonymous request on {pageUrl}");
                return View();
            }

            _logger.LogWarning($"Access denied to user #{user.Email} '{user.Email}' on {pageUrl}");

            return View();
        }
    }
}