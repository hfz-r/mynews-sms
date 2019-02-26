using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.DTOs.Items;
using StockManagementSystem.Api.Helpers;

namespace StockManagementSystem.Api.Validators
{
    public class ItemDtoValidator : BaseDtoValidator<ItemDto>
    {
        public ItemDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper,
            Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper,
            requestJsonDictionary)
        {
            SetStockCodeRule();
        }

        private void SetStockCodeRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(i => i.P_StockCode, "Invalid stock code.", "stock_code");
        }
    }
}