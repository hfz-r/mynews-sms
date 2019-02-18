using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.ModelBinders;

namespace StockManagementSystem.Api.Models.UsersParameters
{
    [ModelBinder(typeof(ParametersModelBinder<UsersSearchParametersModel>))]
    public class UsersSearchParametersModel
    {
        public UsersSearchParametersModel()
        {
            Order = "Id";
            Query = string.Empty;
            Page = Configurations.DefaultPageValue;
            Limit = Configurations.DefaultLimit;
            Fields = string.Empty;
        }

        /// <summary>
        /// Field and direction to order results by (default: id DESC)
        /// </summary>
        [JsonProperty("order")]
        public string Order { get; set; }

        /// <summary>
        /// Text to search users
        /// </summary>
        [JsonProperty("query")]
        public string Query { get; set; }

        /// <summary>
        /// Page to show (default: 1)
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        /// Amount of results (default: 50) (maximum: 250)
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }
    }
}