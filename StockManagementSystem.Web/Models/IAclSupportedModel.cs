using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockManagementSystem.Web.Models
{
    /// <summary>
    /// Represents a model which supports permission control list
    /// </summary>
    public partial interface IAclSupportedModel
    {
        /// <summary>
        /// Gets or sets identifiers of the selected roles
        /// </summary>
        IList<int> SelectedRoleIds { get; set; }

        /// <summary>
        /// Gets or sets items for the all available roles
        /// </summary>
        IList<SelectListItem> AvailableRoles { get; set; }
    } 
}