using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockManagementSystem.Web.Models
{
    /// <summary>
    /// Represents a store supported model
    /// </summary>
    public interface IAppliedStoreSupportedModel
    {
        IList<int> SelectedStoreIds { get; set; }

        IList<SelectListItem> AvailableStores { get; set; }
    } 
}