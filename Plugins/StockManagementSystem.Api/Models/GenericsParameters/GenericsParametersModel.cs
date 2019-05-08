using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.ModelBinders;

namespace StockManagementSystem.Api.Models.GenericsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<GenericsParametersModel>))]
    public class GenericsParametersModel
    {
        public GenericsParametersModel()
        {
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = 0;
            SortColumn = Configurations.DefaultOrder;
            Descending = false;
            Fields = string.Empty;
        }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("since_id")]
        public int SinceId { get; set; }

        [JsonProperty("sort_column")]
        public string SortColumn { get; set; }

        [JsonProperty("descending")]
        public bool Descending { get; set; }

        [JsonProperty("fields")]
        public string Fields { get; set; }
    }
}