using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class BranchMasterRootObject : ISerializableObject
    {
        public BranchMasterRootObject()
        {
            BranchMaster = new List<BranchMasterDto>();
        }

        [JsonProperty("branch")]
        public IList<BranchMasterDto> BranchMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "branch";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(BranchMasterDto);
        }
    }
}
