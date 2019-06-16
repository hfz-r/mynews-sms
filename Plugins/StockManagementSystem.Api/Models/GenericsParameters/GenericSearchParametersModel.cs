using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.ModelBinders;

namespace StockManagementSystem.Api.Models.GenericsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<GenericSearchParametersModel>))]
    public class GenericSearchParametersModel
    {
        public GenericSearchParametersModel()
        {
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SortColumn = Configurations.DefaultOrder;
            Descending = false;
            Fields = string.Empty;
            Count = false;
            Query = string.Empty;
        }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("sort_column")]
        public string SortColumn { get; set; }

        [JsonProperty("descending")]
        public bool Descending { get; set; }

        [JsonProperty("fields")]
        public string Fields { get; set; }

        /// <summary>
        /// Get total count from the search result (default: false)
        /// </summary>
        [JsonProperty("count")]
        public bool Count { get; set; }

        /// <summary>
        /// Comma-separated list of attributes to include in the search
        /// </summary>
        [JsonProperty("query")]
        public string Query { get; set; }
    }
}