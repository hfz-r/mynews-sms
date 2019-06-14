using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.DTOs.Stores;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Stores;
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
        private Mock<IRepository<Store>> _storeRepository;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger>();
            _storeRepository = new Mock<IRepository<Store>>();

            var store1 = new Store
            {
                Id = 1,
                P_BranchNo = 135,
                P_Name = "Store Power Root",
                CreatedOnUtc = DateTime.Parse("2019-06-01T09:09:30.9814175")
            };
            var store2 = new Store
            {
                Id = 2,
                P_BranchNo = 246,
                P_Name = "Store Kacip Fatimah Maa`don",
                CreatedOnUtc = DateTime.Parse("2019-06-10T10:11:09")
            };
            var store3 = new Store
            {
                Id = 3,
                P_BranchNo = 246,
                P_Name = "Store Kosong",
                P_Addr1 = "Lorong Kosong",
                CreatedOnUtc = DateTime.Parse("2019-06-13T01:39:19")
            };

            var dataSet = new List<Store> { store1, store2, store3 }.AsQueryable().BuildMockDbSet();
            _storeRepository.Setup(x => x.Table).Returns(dataSet?.Object);

            var engine = new Mock<Engine>();
            var serviceProvider = new TestGenericApiServiceProvider();

            engine.Setup(x => x.ServiceProvider).Returns(serviceProvider);
            engine.Setup(x => x.ResolveUnregistered(typeof(Repository<Store>))).Returns(_storeRepository.Object);
            EngineContext.Replace(engine.Object);
        }

        [Test]
        public void Can_get_entities_based_on_their_dto()
        {
            var apiService = new GenericApiService<StoreDto>(_logger.Object);

            var result = apiService.GetEntities();

            result.ShouldNotBeNull();
            result.Count.ShouldEqual(2);
        }

        [Test]
        public void Can_get_entity_by_id_based_on_its_dto()
        {
            var apiService = new GenericApiService<StoreDto>(_logger.Object);

            //StoreDto will be use branch_no as its id
            var result = apiService.GetEntityById(135);

            result.Name.ShouldEqual("Store Power Root");
        }

        [Test]
        public void Can_get_entity_count_based_on_its_dto()
        {
            var apiService = new GenericApiService<StoreDto>(_logger.Object);
            var count = apiService.GetEntityCount();

            count.ShouldEqual(2);
        }

        [Test]
        public void Can_search_entity_by_attributes_based_on_its_dto()
        {
            var apiService = new GenericApiService<StoreDto>(_logger.Object);

            var no_query = "";
            var search1 = apiService.Search(no_query);

            search1.Count.ShouldEqual(3);

            var query_with_wrong_attribute = "P_BranchNo:246,P_Namee:Store Kosong";
            var search2 = apiService.Search(query_with_wrong_attribute);

            search2.Count.ShouldEqual(2);

            var query_single_valid = "P_Name:Store Kosong";
            var search3 = apiService.Search(query_single_valid);

            search3.Count.ShouldEqual(1);

            var query_multiple_all_valid = "P_BranchNo:246,P_Name:Store Kosong";
            var search4 = apiService.Search(query_multiple_all_valid);

            search4.Count.ShouldEqual(1);
            search4.First().Address1.ShouldEqual("Lorong Kosong");

            //2019-05-27T09:09:30.9814175
            var query_with_datetime = "createdonutc:-7";
            var search5 = apiService.Search(query_with_datetime);

            search5.Count.ShouldEqual(2);
            search5.Count(x => x.BranchNo == 246).ShouldEqual(2);
        }
    }
}