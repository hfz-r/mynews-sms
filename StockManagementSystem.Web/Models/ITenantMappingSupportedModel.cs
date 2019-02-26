using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockManagementSystem.Web.Models
{
    /// <summary>
    /// Represents the tenant mapping supported model
    /// </summary>
    public partial interface ITenantMappingSupportedModel
    {
        /// <summary>
        /// Gets or sets identifiers of the selected tenants
        /// </summary>
        IList<int> SelectedTenantIds { get; set; }

        /// <summary>
        /// Gets or sets items for the all available tenants
        /// </summary>
        IList<SelectListItem> AvailableTenants { get; set; }
    }
}