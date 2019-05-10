using System;
using Microsoft.AspNetCore.Http;
using Moq;
using StockManagementSystem.Data;

namespace Api.Tests.ServicesTests.Generics
{
    public class TestGenericApiServiceProvider : IServiceProvider
    {
        public TestGenericApiServiceProvider()
        {
            DbContext = new Mock<IDbContext>();
        }

        public Mock<IDbContext> DbContext { get; }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IHttpContextAccessor))
                return new Mock<IHttpContextAccessor>().Object;

            if (serviceType == typeof(IDbContext))
                return DbContext.Object;

            return null;
        }
    }
}