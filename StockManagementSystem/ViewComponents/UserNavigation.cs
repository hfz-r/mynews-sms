using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Factories;

namespace StockManagementSystem.ViewComponents
{
    public class UserNavigationViewComponent : ViewComponent
    {
        private readonly IUserAccountModelFactory _userAccountModelFactory;

        public UserNavigationViewComponent(IUserAccountModelFactory userAccountModelFactory)
        {
            _userAccountModelFactory = userAccountModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int selectedTabId = 0)
        {
            var model = await _userAccountModelFactory.PrepareUserNavigationModel(selectedTabId);

            return View(model);
        }
    }
}