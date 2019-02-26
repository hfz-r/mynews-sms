using FluentValidation.Attributes;
using Newtonsoft.Json;
using StockManagementSystem.Api.Validators;

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

        [JsonProperty("group_id")]
        public int? P_GroupId { get; set; }

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

        [JsonProperty("rec_status")]
        public string P_RecStatus { get; set; }

        [JsonProperty("order_status")]
        public int P_OrderStatus { get; set; }

        [JsonProperty("stock_type")]
        public int P_StockType { get; set; }

        [JsonProperty("variant_1")]
        public string P_Variant1 { get; set; }

        [JsonProperty("variant_2")]
        public string P_Variant2 { get; set; }

        [JsonProperty("vendor_id")]
        public int? VendorId { get; set; }
    }
}