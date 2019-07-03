using System.Collections.Generic;
using FluentValidation;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Api.Validators
{
    public class DeviceDtoValidator : BaseDtoValidator<DeviceDto>
    {
        private readonly IStoreMappingHelper _storeHelper;

        public DeviceDtoValidator(
            IHttpContextAccessor httpContextAccessor,
            IJsonHelper jsonHelper,
            Dictionary<string, object> requestJsonDictionary,
            IStoreMappingHelper storeHelper) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            _storeHelper = storeHelper;

            SetSerialNoRule();
            SetModelNoRule();
            SetStoreIdsRule();
        }

        private void SetSerialNoRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(d => d.SerialNo, "Invalid serial no.", "serial_no");
        }

        private void SetModelNoRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(d => d.ModelNo, "Invalid model no.", "model_no");
        }

        private void SetStoreIdsRule()
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey("store_id"))
            {
                Store store = null;

                RuleFor(x => x.StoreId)
                    .NotNull()
                    .NotEmpty()
                    .Must(id => id > 0)
                    .WithMessage("store_id required")
                    .DependentRules(() => RuleFor(dto => dto.StoreId)
                        .Must(id =>
                        {
                            if (store == null)
                                store = _storeHelper.GetValidStore(id);

                            return store != null;
                        }).WithMessage("invalid store_id")
                    );
            }
        }
    }
}