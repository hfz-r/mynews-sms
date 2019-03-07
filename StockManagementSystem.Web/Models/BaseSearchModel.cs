using System.Collections.Generic;
using StockManagementSystem.Web.Kendoui;

namespace StockManagementSystem.Web.Models
{
    /// <summary>
    /// Represents base search model
    /// </summary>
    public abstract partial class BaseSearchModel : BaseModel, IPagingRequestModel
    {
        public BaseSearchModel()
        {
            //set the default values
            this.Page = 1;
            this.PageSize = 10;
            this.Sort = null;
            this.Filter = null;
        }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public string AvailablePageSizes { get; set; }

        public IList<GridSort> Sort { get; set; }

        public GridFilter Filter { get; set; }

        #region Methods

        public void SetGridPageSize()
        {
            // TODO: implement base-setting
            Page = 1;
            PageSize = 15;
            AvailablePageSizes = "10, 15, 20, 50, 100";
        }

        public void SetPopupGridPageSize()
        {
            // TODO: implement base-setting
            Page = 1;
            PageSize = 10;
            AvailablePageSizes = "10, 15, 20, 50, 100";
        }

        #endregion
    }
}