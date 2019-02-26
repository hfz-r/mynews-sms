using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using StockManagementSystem.Api.Validators;

namespace StockManagementSystem.Api.DTOs.Devices
{
    [JsonObject(Title = "device")]
    [Validator(typeof(DeviceDtoValidator))]
    public class DeviceDto : BaseDto
    {
        [JsonProperty("serial_no")]
        public string SerialNo { get; set; }

        [JsonProperty("model_no")]
        public string ModelNo { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("token_id")]
        public string TokenId { get; set; }

        [JsonProperty("store_id")]
        public int StoreId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        //[JsonProperty("store")]
        //public StoreDto Store { get; set; }

        [JsonProperty("limited_to_tenants")]
        public bool LimitedToTenants { get; set; }

        [JsonProperty("tenant_ids")]
        public List<int> TenantIds { get; set; }
    }
}