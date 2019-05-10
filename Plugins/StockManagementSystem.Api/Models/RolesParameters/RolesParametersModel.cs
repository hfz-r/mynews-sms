using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.ModelBinders;

namespace StockManagementSystem.Api.Models.RolesParameters
{
    [ModelBinder(typeof(ParametersModelBinder<RolesParametersModel>))]
    public class RolesParametersModel
    {
        public RolesParametersModel()
        {
            Ids = null;
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = Configurations.DefaultSinceId;
            Fields = string.Empty;
            PermissionIds = null;
        }

        /// <summary>
        /// A comma-separated list of role ids
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
        /// Comma-separated list of permission ids for filter role
        /// </summary>
        [JsonProperty("permission_ids")]
        public List<int> PermissionIds { get; set; }
    }
}