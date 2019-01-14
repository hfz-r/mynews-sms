using Microsoft.AspNetCore.Http;

namespace StockManagementSystem.Core
{
    public interface IWebHelper
    {
        string GetUrlReferrer();

        string GetCurrentIpAddress();

        string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);

        bool IsCurrentConnectionSecured();

        string GetAppHost(bool useSsl);

        string GetAppLocation(bool? useSsl = null);

        bool IsStaticResource();

        string CurrentRequestProtocol { get; }

        string GetRawUrl(HttpRequest request);
    }
}