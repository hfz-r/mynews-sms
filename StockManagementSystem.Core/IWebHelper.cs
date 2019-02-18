using Microsoft.AspNetCore.Http;

namespace StockManagementSystem.Core
{
    public interface IWebHelper
    {
        string GetUrlReferrer();

        string GetCurrentIpAddress();

        string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);

        bool IsCurrentConnectionSecured();

        string GetStoreHost(bool useSsl);

        string GetStoreLocation(bool? useSsl = null);

        string ModifyQueryString(string url, string key, params string[] values);

        string RemoveQueryString(string url, string key, string value = null);

        T QueryString<T>(string name);

        bool IsStaticResource();

        void RestartAppDomain(bool makeRedirect = false);

        string CurrentRequestProtocol { get; }

        string GetRawUrl(HttpRequest request);
    }
}