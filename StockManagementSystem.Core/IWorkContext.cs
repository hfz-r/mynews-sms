using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Core
{
    public interface IWorkContext
    {
        User CurrentUser { get; set; }
    }
}