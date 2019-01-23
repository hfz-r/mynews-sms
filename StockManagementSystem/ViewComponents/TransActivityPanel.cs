using Microsoft.AspNetCore.Mvc;

namespace StockManagementSystem.ViewComponents
{
    public class TransActivityPanelViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}