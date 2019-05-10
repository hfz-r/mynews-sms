using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.ModelBinders;

namespace StockManagementSystem.Api.Models.PushNotificationsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<PushNotificationsParametersModel>))]
    public class PushNotificationsParametersModel
    {
        public PushNotificationsParametersModel()
        {
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = 0;
            Fields = string.Empty;
            CreatedAtMax = null;
            CreatedAtMin = null;
            StoreIds = null;
        }

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
        /// Comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }

        /// <summary>
        /// Show push notifications created after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_min")]
        public DateTime? CreatedAtMin { get; set; }

        /// <summary>
        /// Show push notifications created before date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_max")]
        public DateTime? CreatedAtMax { get; set; }


        /// <summary>
        /// Comma-separated list of store ids for filter push notifications
        /// </summary>
        [JsonProperty("store_ids")]
        public List<int> StoreIds { get; set; }
    }
}