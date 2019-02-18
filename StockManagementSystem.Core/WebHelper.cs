using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Http;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Core
{
    public class WebHelper : IWebHelper
    {
        private readonly HostingConfig _hostingConfig;
        private readonly IFileProviderHelper _fileProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebHelper(HostingConfig hostingConfig, IFileProviderHelper fileProvider, IHttpContextAccessor httpContextAccessor)
        {
            _hostingConfig = hostingConfig;
            _fileProvider = fileProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Utilities

        protected virtual bool IsRequestAvailable()
        {
            if (_httpContextAccessor?.HttpContext == null)
                return false;

            try
            {
                if (_httpContextAccessor.HttpContext.Request == null)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        protected virtual bool TryWriteWebConfig()
        {
            try
            {
                _fileProvider.SetLastWriteTimeUtc(_fileProvider.MapPath(InfrastructureDefaults.WebConfigPath), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        public virtual string GetUrlReferrer()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            return _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer];
        }

        public virtual string GetCurrentIpAddress()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            var result = string.Empty;
            try
            {
                //first try to get IP address from the forwarded header
                if (_httpContextAccessor.HttpContext.Request.Headers != null)
                {
                    var forwardedHttpHeaderKey = HttpDefaults.XForwardedForHeader;
                    if (!string.IsNullOrEmpty(_hostingConfig.ForwardedHttpHeader))
                    {
                        forwardedHttpHeaderKey = _hostingConfig.ForwardedHttpHeader;
                    }

                    var forwardedHeader = _httpContextAccessor.HttpContext.Request.Headers[forwardedHttpHeaderKey];
                    if (!StringValues.IsNullOrEmpty(forwardedHeader))
                        result = forwardedHeader.FirstOrDefault();
                }

                //if this header not exists try get connection remote IP address
                if (string.IsNullOrEmpty(result) && _httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
                    result = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            catch
            {
                return string.Empty;
            }

            //some of the validation
            if (result != null && result.Equals(IPAddress.IPv6Loopback.ToString(), StringComparison.InvariantCultureIgnoreCase))
                result = IPAddress.Loopback.ToString();

            //"TryParse" doesn't support IPv4 with port number
            if (IPAddress.TryParse(result ?? string.Empty, out var ip))
            {
                //IP address is valid 
                result = ip.ToString();
            }
            else if (!string.IsNullOrEmpty(result))
            {
                //remove port
                result = result.Split(':').FirstOrDefault();
            }

            return result;
        }

        public virtual string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false)
        {
            if (!IsRequestAvailable())
                return string.Empty;

            // get app location
            var storeLocation = GetStoreLocation(useSsl ?? IsCurrentConnectionSecured());

            //add local path to the URL
            var pageUrl = $"{storeLocation.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.Path}";

            //add query string to the URL
            if (includeQueryString)
                pageUrl = $"{pageUrl}{_httpContextAccessor.HttpContext.Request.QueryString}";

            //whether to convert the URL to lower case
            if (lowercaseUrl)
                pageUrl = pageUrl.ToLowerInvariant();

            return pageUrl;
        }

        public virtual bool IsCurrentConnectionSecured()
        {
            if (!IsRequestAvailable())
                return false;

            //use HTTP_CLUSTER_HTTPS?
            if (_hostingConfig.UseHttpClusterHttps)
                return _httpContextAccessor.HttpContext.Request.Headers[HttpDefaults.HttpClusterHttpsHeader].ToString().Equals("on", StringComparison.OrdinalIgnoreCase);

            //use HTTP_X_FORWARDED_PROTO?
            if (_hostingConfig.UseHttpXForwardedProto)
                return _httpContextAccessor.HttpContext.Request.Headers[HttpDefaults.HttpXForwardedProtoHeader].ToString().Equals("https", StringComparison.OrdinalIgnoreCase);

            return _httpContextAccessor.HttpContext.Request.IsHttps;
        }

        public virtual string GetStoreHost(bool useSsl)
        {
            if (!IsRequestAvailable())
                return string.Empty;

            var hostHeader = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
            if (StringValues.IsNullOrEmpty(hostHeader))
                return string.Empty;

            //add scheme to the URL
            var storeHost = $"{(useSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp)}{Uri.SchemeDelimiter}{hostHeader.FirstOrDefault()}";

            //ensure that host is ended with slash
            storeHost = $"{storeHost.TrimEnd('/')}/";

            return storeHost;
        }

        public virtual string GetStoreLocation(bool? useSsl = null)
        {
            var storeLocation = string.Empty;

            // get app host
            var storeHost = GetStoreHost(useSsl ?? IsCurrentConnectionSecured());
            if (!string.IsNullOrEmpty(storeHost))
            {
                //add application path base if exists
                storeLocation = IsRequestAvailable() ? $"{storeHost.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.PathBase}" : storeHost;
            }

            if (string.IsNullOrEmpty(storeHost) && DataSettingsManager.DatabaseIsInstalled)
            {
                storeLocation = EngineContext.Current.Resolve<IStoreContext>().CurrentStore?.Url ??
                                throw new Exception("Current store cannot be loaded");
            }
            
            //ensure that URL is ended with slash
            storeLocation = $"{storeLocation.TrimEnd('/')}/";

            return storeLocation;
        }

        public virtual bool IsStaticResource()
        {
            if (!IsRequestAvailable())
                return false;

            string path = _httpContextAccessor.HttpContext.Request.Path;

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            return contentTypeProvider.TryGetContentType(path, out string _);
        }

        public virtual string ModifyQueryString(string url, string key, params string[] values)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            if (string.IsNullOrEmpty(key))
                return url;

            //get current query parameters
            var uri = new Uri(url);
            var queryParameters = QueryHelpers.ParseQuery(uri.Query);

            //and add passed one
            queryParameters[key] = string.Join(",", values);

            var queryBuilder = new QueryBuilder(queryParameters
                .ToDictionary(parameter => parameter.Key, parameter => parameter.Value.FirstOrDefault()?.ToString() ?? string.Empty));

            //create new URL with passed query parameters
            url = $"{uri.GetLeftPart(UriPartial.Path)}{queryBuilder.ToQueryString()}{uri.Fragment}";

            return url;
        }

        public virtual string RemoveQueryString(string url, string key, string value = null)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            if (string.IsNullOrEmpty(key))
                return url;

            //get current query parameters
            var uri = new Uri(url);
            var queryParameters = QueryHelpers.ParseQuery(uri.Query)
                .SelectMany(parameter => parameter.Value, (pair, s) => new KeyValuePair<string, string>(pair.Key, s))
                .ToList();

            if (!string.IsNullOrEmpty(value))
            {
                //remove a specific query parameter value if it's passed
                queryParameters.RemoveAll(parameter =>
                    parameter.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) &&
                    parameter.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                //remove query parameter by the key
                queryParameters.RemoveAll(parameter =>
                    parameter.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            }

            //create new URL without passed query parameters
            url = $"{uri.GetLeftPart(UriPartial.Path)}{new QueryBuilder(queryParameters).ToQueryString()}{uri.Fragment}";

            return url;
        }

        public virtual T QueryString<T>(string name)
        {
            if (!IsRequestAvailable())
                return default(T);

            if (StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Query[name]))
                return default(T);

            return CommonHelper.To<T>(_httpContextAccessor.HttpContext.Request.Query[name].ToString());
        }

        public virtual void RestartAppDomain(bool makeRedirect = false)
        {
            //"touch" web.config to force restart
            var success = TryWriteWebConfig();
            if (!success)
            {
                throw new DefaultException(
                    "SMS needs to be restarted due to a configuration change, but was unable to do so." +
                    Environment.NewLine +
                    "To prevent this issue in the future, a change to the web server configuration is required:" +
                    Environment.NewLine +
                    "- run the application in a full trust environment, or" + Environment.NewLine +
                    "- give the application write access to the 'web.config' file.");
            }
        }

        public virtual string CurrentRequestProtocol => IsCurrentConnectionSecured() ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

        public virtual string GetRawUrl(HttpRequest request)
        {
            var rawUrl = request.HttpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;
            if (string.IsNullOrEmpty(rawUrl))
                rawUrl = $"{request.PathBase}{request.Path}{request.QueryString}";

            return rawUrl;
        }
    }
}