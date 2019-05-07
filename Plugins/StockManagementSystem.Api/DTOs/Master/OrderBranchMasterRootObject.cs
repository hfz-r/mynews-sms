using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class OrderBranchMasterRootObject : ISerializableObject
    {
        public OrderBranchMasterRootObject()
        {
            OrderBranchMaster = new List<OrderBranchMasterDto>();
        }

        [JsonProperty("orderBranch")]
        public IList<OrderBranchMasterDto> OrderBranchMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "orderBranch";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(OrderBranchMasterDto);
        }
    }
}
