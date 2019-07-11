using System.Collections.Generic;
using System.Net.Http;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.DTOs.Transactions;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Api.Validators
{
    public class TransactionDtoValidator : BaseDtoValidator<TransactionDto>
    {
        private readonly IStoreMappingHelper _storeHelper;

        public TransactionDtoValidator(
            IHttpContextAccessor httpContextAccessor,
            IJsonHelper jsonHelper,
            Dictionary<string, object> requestJsonDictionary,
            IStoreMappingHelper storeHelper) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            _storeHelper = storeHelper;

            SetStockCodeRule();
            SetDeviceSerialNoRule();
            SetBranchNoRule();
        }

        private void SetStockCodeRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(tnx => tnx.P_StockCode, "Invalid stock code.", "stock_code");
        }

        private void SetDeviceSerialNoRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(tnx => tnx.DeviceSerialNo, "Invalid device serial no", "device_serial_no");
        }

        private void SetBranchNoRule()
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey("branch_no"))
            {
                Store store = null;

                RuleFor(x => x.P_BranchNo)
                    .NotNull()
                    .NotEmpty()
                    .Must(id => id > 0)
                    .WithMessage("branch_no required")
                    .DependentRules(() => RuleFor(dto => dto.P_BranchNo)
                        .Must(id =>
                        {
                            if (store == null)
                                store = _storeHelper.GetValidStore(id.GetValueOrDefault());

                            return store != null;
                        }).WithMessage("invalid branch_no")
                    );
            }
        }
    }
}