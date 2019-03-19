using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Web.Kendoui;

namespace StockManagementSystem.Web.Models
{
    /// <summary>
    /// Represents base search model
    /// </summary>
    public abstract partial class BaseSearchModel : BaseModel, IPagingRequestModel
    {
        protected BaseSearchModel()
        {
            Page = 1;
            PageSize = 10;
            Sort = null;
            Filter = null;
        }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public string AvailablePageSizes { get; set; }

        public IList<GridSort> Sort { get; set; }

        public GridFilter Filter { get; set; }

        #region Methods

        public void SetGridPageSize()
        {
            var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

            Page = 1;
            PageSize = commonSettings.DefaultGridPageSize;
            AvailablePageSizes = commonSettings.GridPageSizes;
        }

        public void SetPopupGridPageSize()
        {
            var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

            Page = 1;
            PageSize = commonSettings.PopupGridPageSize;
            AvailablePageSizes = commonSettings.GridPageSizes;
        }

        #endregion
    }
}