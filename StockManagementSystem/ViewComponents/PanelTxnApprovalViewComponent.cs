using Microsoft.AspNetCore.Mvc;

namespace StockManagementSystem.ViewComponents
{
    [ViewComponent(Name = "PanelTxnApprovalComponent")]
    public class PanelTxnApprovalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
