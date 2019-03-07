using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Stores
{
    //TODO: AssignUserValidator
    public class StoreModel : BaseEntityModel
    {
        public StoreModel()
        {
            AvailableAreaCodes = new List<SelectListItem>();
            AvailableCities = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();

            UserStoreSearchModel = new UserStoreSearchModel();
        }

        [Display(Name = "Branch no.")]
        public int BranchNo { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Area code")]
        public string AreaCode { get; set; }
        public IList<SelectListItem> AvailableAreaCodes { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }
        public IList<SelectListItem> AvailableCities { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public string Country { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public int CountUserStore { get; set; }

        public UserStoreSearchModel UserStoreSearchModel { get; set; }
    }
}