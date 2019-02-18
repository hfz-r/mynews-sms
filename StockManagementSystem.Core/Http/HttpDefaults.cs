namespace StockManagementSystem.Core.Http
{
    /// <summary>
    /// Represents default values related to HTTP features
    /// </summary>
    public static class HttpDefaults
    {
        /// <summary>
        /// Gets a request path to the install URL
        /// </summary>
        public static string InstallPath => "install";

        /// <summary>
        /// Gets a request path to the keep alive URL
        /// </summary>
        public static string KeepAlivePath => "keepalive/index";

        /// <summary>
        /// Gets the name of HTTP_CLUSTER_HTTPS header
        /// </summary>
        public static string HttpClusterHttpsHeader => "HTTP_CLUSTER_HTTPS";

        /// <summary>
        /// Gets the name of HTTP_X_FORWARDED_PROTO header
        /// </summary>
        public static string HttpXForwardedProtoHeader => "X-Forwarded-Proto";

        /// <summary>
        /// Gets the name of X-FORWARDED-FOR header
        /// </summary>
        public static string XForwardedForHeader => "X-FORWARDED-FOR";
    }
}