using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.LicenseManager.Validators;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.LicenseManager.Models
{
    [Validator(typeof(LicenseValidator))]
    public class LicenseModel : BaseEntityModel
    {
        public LicenseModel()
        {
            AvailableLicenseType = new List<SelectListItem>();

            DeviceLicenseSearchModel = new DeviceLicenseSearchModel();
        }

        [Display(Name = "License name")]
        public string Name { get; set; }

        [Display(Name = "License email")]
        public string Email { get; set; }

        [Display(Name = "Expiration date")]
        [UIHint("DateTimeNullable")]
        public DateTime? ExpiryDate { get; set; }

        //license is generated?
        [Display(Name = "Generated")]
        public bool Generated { get; set; }

        //license type
        [Display(Name = "License type")]
        public int LicenseTypeId { get; set; }
        [Display(Name = "License type")]
        public string LicenseType { get; set; }

        public IList<SelectListItem> AvailableLicenseType { get; set; }

        public int CountDevices { get; set; }

        public int DownloadId { get; set; }

        public DeviceLicenseSearchModel DeviceLicenseSearchModel { get; set; }
    }
}