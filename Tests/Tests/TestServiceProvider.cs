using System;
using Microsoft.AspNetCore.Http;
using Moq;
using StockManagementSystem.Core;
using StockManagementSystem.Services.Common;

namespace Tests
{
    public class TestServiceProvider : IServiceProvider
    {
        public TestServiceProvider()
        {
            GenericAttributeService = new Mock<IGenericAttributeService>();
            WorkContext = new Mock<IWorkContext>();
        }

        public Mock<IWorkContext> WorkContext { get; }
        public Mock<IGenericAttributeService> GenericAttributeService { get; }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IHttpContextAccessor))
                return new Mock<IHttpContextAccessor>().Object;

            if (serviceType == typeof(IWorkContext))
                return WorkContext.Object;

            if (serviceType == typeof(IGenericAttributeService))
                return GenericAttributeService.Object;

            return null;
        }
    }
}