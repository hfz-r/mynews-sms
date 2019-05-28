using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.TransporterTransaction
{
    public class TransporterTransactionRootObject : ISerializableObject
    {
        public TransporterTransactionRootObject()
        {
            TransporterTransaction = new List<TransporterTransactionDto>();   
        }

        [JsonProperty("transporter_transaction")]
        public IList<TransporterTransactionDto> TransporterTransaction { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "transporter_transaction";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(TransporterTransactionDto);
        }
    }
}