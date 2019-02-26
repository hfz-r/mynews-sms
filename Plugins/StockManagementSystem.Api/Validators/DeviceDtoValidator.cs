using System.Collections.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.Helpers;

namespace StockManagementSystem.Api.Validators
{
    public class DeviceDtoValidator : BaseDtoValidator<DeviceDto>
    {
        public DeviceDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper,
            Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper,
            requestJsonDictionary)
        {
            SetSerialNoRule();
            SetModelNoRule();
            //TODO: validate storeId
        }

        private void SetSerialNoRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(d => d.SerialNo, "Invalid serial no.", "serial_no");
        }

        private void SetModelNoRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(d => d.ModelNo, "Invalid model no.", "model_no");
        }
    }
}