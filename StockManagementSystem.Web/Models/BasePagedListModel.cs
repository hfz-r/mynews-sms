using System.Collections.Generic;

namespace StockManagementSystem.Web.Models
{
    /// <summary>
    /// Represents the base paged list model (implementation for KendoUI grids)
    /// </summary>
    public abstract partial class BasePagedListModel<T> : BaseModel, IPagedModel<T> where T :  BaseModel
    {
        /// <summary>
        /// Gets or sets data records
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Gets or sets total records number
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets an extra data
        /// </summary>
        public object ExtraData { get; set; }

        /// <summary>
        /// Gets or sets an errors
        /// </summary>
        public object Errors { get; set; }
    }
}