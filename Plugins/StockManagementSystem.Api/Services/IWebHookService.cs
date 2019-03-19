using Microsoft.AspNetCore.WebHooks;

namespace StockManagementSystem.Api.Services
{
    public interface IWebHookService
    {
        IWebHookFilterManager GetWebHookFilterManager();

        IWebHookManager GetWebHookManager();

        IWebHookSender GetWebHookSender();

        IWebHookStore GetWebHookStore();
    }
}