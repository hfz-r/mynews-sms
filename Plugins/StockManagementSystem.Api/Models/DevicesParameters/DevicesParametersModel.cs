using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.ModelBinders;

namespace StockManagementSystem.Api.Models.DevicesParameters
{
    [ModelBinder(typeof(ParametersModelBinder<DevicesParametersModel>))]
    public class DevicesParametersModel
    {
        public DevicesParametersModel()
        {
            Ids = null;
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = Configurations.DefaultSinceId;
            Fields = string.Empty;
            CreatedAtMin = null;
            CreatedAtMax = null;
            Status = string.Empty;
        }

        /// <summary>
        /// A comma-separated list of device ids
        /// </summary>
        [JsonProperty("ids")]
        public List<int> Ids { get; set; }

        /// <summary>
        /// Amount of results (default: 50) (maximum: 250)
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Page to show (default: 1)
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        /// Restrict results to after the specified ID
        /// </summary>
        [JsonProperty("since_id")]
        public int SinceId { get; set; }

        /// <summary>
        /// comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }

        /// <summary>
        /// Show devices created after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_min")]
        public DateTime? CreatedAtMin { get; set; }

        /// <summary>
        /// Show devices created before date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_max")]
        public DateTime? CreatedAtMax { get; set; }

        /// <summary>
        /// <ul>
        /// <li>0 - offline (default)</li>
        /// <li>1 - Show online status</li>
        /// <li>2 - Show out of radar status</li>
        /// <li>N/A - Unknown status</li>
        /// </ul>
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}