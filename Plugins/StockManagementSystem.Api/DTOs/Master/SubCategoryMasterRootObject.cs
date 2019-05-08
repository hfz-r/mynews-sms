using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class SubCategoryMasterRootObject : ISerializableObject
    {
        public SubCategoryMasterRootObject()
        {
            SubCategoryMaster = new List<SubCategoryMasterDto>();
        }

        [JsonProperty("subCategory")]
        public IList<SubCategoryMasterDto> SubCategoryMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "subCategory";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(SubCategoryMasterDto);
        }
    }
}
