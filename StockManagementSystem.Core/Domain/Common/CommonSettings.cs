using StockManagementSystem.Core.Configuration;

namespace StockManagementSystem.Core.Domain.Common
{
    public class CommonSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display a warning if java-script is disabled
        /// </summary>
        public bool DisplayJavaScriptDisabledWarning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 404 errors (page or file not found) should be logged
        /// </summary>
        public bool Log404Errors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to compress response (gzip by default). 
        /// May disable it if have an active IIS Dynamic Compression Module configured at server
        /// </summary>
        public bool UseResponseCompression { get; set; }

        /// <summary>
        /// Gets or sets a value of "Cache-Control" header value for static content
        /// </summary>
        public string StaticFilesCacheControl { get; set; }

        /// <summary>
        /// Gets or sets a picture identifier of the logo.
        /// </summary>
        public int LogoPictureId { get; set; }

        /// <summary>
        /// Default grid page size
        /// </summary>
        public int DefaultGridPageSize { get; set; }

        /// <summary>
        /// Popup grid page size (for popup pages)
        /// </summary>
        public int PopupGridPageSize { get; set; }

        /// <summary>
        /// A comma-separated list of available grid page sizes
        /// </summary>
        public string GridPageSizes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use IsoDateFormat in JSON results (used for avoiding issue with dates in KendoUI grids)
        /// </summary>
        public bool UseIsoDateFormatInJsonResult { get; set; }

        /// <summary>
        /// Indicates whether to use nested setting design
        /// </summary>
        public bool UseNestedSetting { get; set; }
    }
}