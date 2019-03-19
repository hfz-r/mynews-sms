using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Factories;

namespace StockManagementSystem.ViewComponents
{
    public class HeaderLinksViewComponent : ViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public HeaderLinksViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _commonModelFactory.PrepareHeaderLinksModel();
            return View(model);
        }
    }
}