using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class MainCategoryMasterRootObject : ISerializableObject
    {
        public MainCategoryMasterRootObject()
        {
            MainCategoryMaster = new List<MainCategoryMasterDto>();
        }

        [JsonProperty("mainCategory")]
        public IList<MainCategoryMasterDto> MainCategoryMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "mainCategory";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(MainCategoryMasterDto);
        }
    }
}
