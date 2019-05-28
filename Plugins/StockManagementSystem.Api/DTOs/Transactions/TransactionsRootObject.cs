using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Transactions
{
    public class TransactionsRootObject : ISerializableObject
    {
        public TransactionsRootObject()
        {
            Transactions = new List<TransactionDto>();
        }

        [JsonProperty("transactions")]
        public IList<TransactionDto> Transactions { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "transactions";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(TransactionDto);
        }
    }
}