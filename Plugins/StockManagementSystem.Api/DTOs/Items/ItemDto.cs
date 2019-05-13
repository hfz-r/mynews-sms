using FluentValidation.Attributes;
using Newtonsoft.Json;
using StockManagementSystem.Api.Validators;
using System;

namespace StockManagementSystem.Api.DTOs.Items
{
    [JsonObject(Title = "item")]
    [Validator(typeof(ItemDtoValidator))]
    public class ItemDto : BaseDto
    {
        [JsonProperty("stock_code")]
        public string P_StockCode { get; set; }

        [JsonProperty("desc")]
        public string P_Desc { get; set; }

        [JsonProperty("sub_category_code")]
        public int P_SubCategoryID { get; set; }

        [JsonProperty("price_1")]
        public double? P_SPrice1 { get; set; }

        [JsonProperty("price_2")]
        public double? P_SPrice2 { get; set; }

        [JsonProperty("price_3")]
        public double? P_SPrice3 { get; set; }

        [JsonProperty("price_4")]
        public double? P_SPrice4 { get; set; }

        [JsonProperty("price_5")]
        public double? P_SPrice5 { get; set; }

        [JsonProperty("price_6")]
        public double? P_SPrice6 { get; set; }

        [JsonProperty("price_7")]
        public double? P_SPrice7 { get; set; }

        [JsonProperty("price_8")]
        public double? P_SPrice8 { get; set; }

        [JsonProperty("price_9")]
        public double? P_SPrice9 { get; set; }

        [JsonProperty("price_10")]
        public double? P_SPrice10 { get; set; }

        [JsonProperty("price_11")]
        public double? P_SPrice11 { get; set; }

        [JsonProperty("price_12")]
        public double? P_SPrice12 { get; set; }

        [JsonProperty("price_13")]
        public double? P_SPrice13 { get; set; }

        [JsonProperty("price_14")]
        public double? P_SPrice14 { get; set; }

        [JsonProperty("price_15")]
        public double? P_SPrice15 { get; set; }

        [JsonProperty("modify_dt")]
        public DateTime? P_ModifyDT { get; set; }

        [JsonProperty("order_status")]
        public int P_OrderStatus { get; set; }

        [JsonProperty("display_shelf_life")]
        public int P_DisplayShelfLife { get; set; }
    }
}