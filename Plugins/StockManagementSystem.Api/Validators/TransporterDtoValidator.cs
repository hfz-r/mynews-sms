using System.Collections.Generic;
using System.Net.Http;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.DTOs.TransporterTransaction;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Api.Validators
{
    public class TransporterDtoValidator : BaseDtoValidator<TransporterTransactionDto>
    {
        private readonly IStoreMappingHelper _storeHelper;

        public TransporterDtoValidator(
            IHttpContextAccessor httpContextAccessor, 
            IJsonHelper jsonHelper, 
            Dictionary<string, object> requestJsonDictionary,
            IStoreMappingHelper storeHelper) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            _storeHelper = storeHelper;

            SetDriverNameRule();
            SetVehiclePlateNoRule();
            SetBranchNoRule();
        }

        private void SetDriverNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(transport => transport.DriverName, "Invalid driver name.", "driver_name");
        }

        private void SetVehiclePlateNoRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(transport => transport.VehiclePlateNo, "Invalid vehicle plate no.", "vehicle_plate_no");
        }

        private void SetBranchNoRule()
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey("branch_no"))
            {
                Store store = null;

                RuleFor(x => x.BranchNo)
                    .NotNull()
                    .NotEmpty()
                    .Must(id => id > 0)
                    .WithMessage("branch_no required")
                    .DependentRules(() => RuleFor(dto => dto.BranchNo)
                        .Must(id =>
                        {
                            if (store == null)
                                store = _storeHelper.GetValidStore(id);

                            return store != null;
                        }).WithMessage("invalid branch_no")
                    );
            }
        }
    }
}