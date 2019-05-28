using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.DTOs.Transactions;
using StockManagementSystem.Api.Infrastructure.Mapper;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Transactions;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Data;
using StockManagementSystem.Services.Logging;
using Tests;

namespace Api.Tests.ServicesTests.Generics
{
    [TestFixture]
    public class GenericApiServiceTests
    {
        private Mock<ILogger> _logger;
        private Mock<IRepository<Transaction>> _transactionRepository;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger>();
            _transactionRepository = new Mock<IRepository<Transaction>>();

            var transaction1 = new Transaction
            {
                Id = 1,
                P_BranchNo = 135,
                P_StockCode = "Stock1"
            };
            var transaction2 = new Transaction
            {
                Id = 2,
                P_BranchNo = 246,
                P_StockCode = "Stock2"
            };

            var dataSet = new List<Transaction> { transaction1, transaction2 }.AsQueryable().BuildMockDbSet();
            _transactionRepository.Setup(x => x.Table).Returns(dataSet?.Object);

            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Transaction, TransactionDto>();

            var engine = new Mock<Engine>();
            var serviceProvider = new TestGenericApiServiceProvider();

            engine.Setup(x => x.ServiceProvider).Returns(serviceProvider);
            engine.Setup(x => x.ResolveUnregistered(typeof(Repository<Transaction>))).Returns(_transactionRepository.Object);
            EngineContext.Replace(engine.Object);
        }

        [Test]
        public void Can_get_entities_based_on_their_dto()
        {
            var apiService = new GenericApiService<TransactionDto>(_logger.Object);

            var result = apiService.GetEntities();

            result.ShouldNotBeNull();
            result.Count.ShouldEqual(2);
        }

        [Test]
        public void Can_get_entity_by_id_based_on_its_dto()
        {
            var apiService = new GenericApiService<TransactionDto>(_logger.Object);

            var result = apiService.GetEntityById(1);

            result.P_BranchNo.ShouldEqual(135);
        }

        [Test]
        public void Can_get_entity_count_based_on_its_dto()
        {
            var apiService = new GenericApiService<TransactionDto>(_logger.Object);
            var count = apiService.GetEntityCount();

            count.ShouldEqual(2);
        }
    }
}