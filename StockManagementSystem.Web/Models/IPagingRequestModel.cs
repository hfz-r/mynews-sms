using System.Collections.Generic;
using StockManagementSystem.Web.Kendoui;

namespace StockManagementSystem.Web.Models
{
    /// <summary>
    /// Represents a paging request model
    /// </summary>
    public partial interface IPagingRequestModel
    {
        /// <summary>
        /// Gets or sets a page number
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// Gets or sets a page size
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// Get or sets server-side grid sorting
        /// </summary>
        IList<GridSort> Sort { get; set; }

        /// <summary>
        /// Gets or sets server-side grid filter
        /// </summary>
        GridFilter Filter { get; set; }
    }
}