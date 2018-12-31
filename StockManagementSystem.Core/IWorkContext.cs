using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Core
{
    public interface IWorkContext
    {
        User CurrentUser { get; set; }
    }
}