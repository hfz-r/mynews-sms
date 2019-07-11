using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.DTOs.PushNotifications;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Api.Validators
{
    public class PushNotificationDtoValidator : BaseDtoValidator<PushNotificationDto>
    {
        private readonly IStoreMappingHelper _storeHelper;

        public PushNotificationDtoValidator(
            IHttpContextAccessor httpContextAccessor, 
            IJsonHelper jsonHelper, 
            Dictionary<string, object> requestJsonDictionary,
            IStoreMappingHelper storeHelper) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            _storeHelper = storeHelper;

            SetTitleRule();
            SetStoreIdsRule();
        }

        private void SetTitleRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(pn => pn.Title, "Invalid title", "title");
        }

        private void SetStoreIdsRule()
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey("store_ids"))
            {
                IList<Store> stores = null;

                RuleFor(x => x.StoreIds)
                    .NotNull()
                    .Must(r => r.Count > 0)
                    .WithMessage("store_ids required")
                    .DependentRules(() => RuleFor(dto => dto.StoreIds)
                        .Must(storeIds =>
                        {
                            if (stores == null)
                                stores = _storeHelper.GetValidStores(storeIds);

                            return stores.Any();
                        })
                        .WithMessage("invalid store_ids")
                    );
            }
        }
    }
}