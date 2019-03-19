using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StockManagementSystem.ViewComponents
{
    public class SearchBoxViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}