using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.DTOs.TransporterTransaction;
using StockManagementSystem.Api.Infrastructure.Mapper;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Transactions;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Helpers;
using Tests;

namespace Api.Tests.Infrastructure
{
    [TestFixture]
    public class CustomResolverTests
    {
        private Mock<IRepository<TransporterTransaction>> _transporterRepository;
        private Mock<IWorkContext> _workContext;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Mock<ISettingService> _settingService;
        private IDateTimeHelper _dateTimeHelper;
        private TransporterTransactionApiService _transporterApiService;
        private IList<TransporterTransaction> _transporters;

        [SetUp]
        public void SetUp()
        {
            _transporterRepository = new Mock<IRepository<TransporterTransaction>>();
            _workContext = new Mock<IWorkContext>();
            _genericAttributeService = new Mock<IGenericAttributeService>();
            _settingService = new Mock<ISettingService>();

            _transporters = new List<TransporterTransaction>
            {
                new TransporterTransaction
                {
                    Id = 1,
                    DriverName = "Tino Garikhan",
                    P_StaffNo = 11,
                    CreatedOnUtc = DateTime.UtcNow
                },
                new TransporterTransaction
                {
                    Id = 2,
                    DriverName = "Pak Hangtu",
                    P_StaffNo = 22,
                    CreatedOnUtc = DateTime.UtcNow
                },
                new TransporterTransaction
                {
                    Id = 3,
                    DriverName = "Alabumi Adigoon",
                    P_StaffNo = 33,
                    CreatedOnUtc = DateTime.UtcNow
                },
            };

            var ds = _transporters.BuildMockDbSet();
            _transporterRepository.Setup(x => x.Table).Returns(ds.Object);

            _transporterApiService = new TransporterTransactionApiService(_transporterRepository.Object);

            _dateTimeHelper = new DateTimeHelper(new DateTimeSettings(), _genericAttributeService.Object,
                _settingService.Object, _workContext.Object);

            AutoMapperApiConfiguration.MapperConfigurationExpression
                .CreateMap<TransporterTransaction, TransporterTransactionDto>()
                .ForMember(model => model.CreatedOnLocal, opt => opt.MapFrom(new DateTimeResolver(_dateTimeHelper)));
        }

        [Test]
        public void Collection_should_return_local_datetime()
        {
            var entity = _transporterApiService.GetTransporterTransaction();
            var dto = entity.Select(d => d.ToDto()).ToList();

            dto.First().CreatedOnLocal.ShouldNotBeNull();

            var dt = Convert.ToDateTime(dto.First().CreatedOnLocal).ToUniversalTime();

            dto.First().CreatedOnUtc?.Date.Equals(dt.Date).ShouldEqual(true);
        }
    }
}