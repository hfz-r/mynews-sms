using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
        private Mock<IDbContext> _dbContext;
        private Mock<DbSet<Store>> _dbSet;
        private Mock<IRepository<Store>> _storeRepository;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger>();
            _dbContext = new Mock<IDbContext>();
            _dbSet = new Mock<DbSet<Store>>();
            _storeRepository = new Mock<IRepository<Store>>();

            var store1 = new Store
            {
                Id = 1,
                P_BranchNo = 135,
                P_Name = "Store Power Root",
                CreatedOnUtc = DateTime.Parse("2019-07-10T09:09:30.9814175")
            };
            var store2 = new Store
            {
                Id = 2,
                P_BranchNo = 246,
                P_Name = "Store Kacip Fatimah Maa`don",
                CreatedOnUtc = DateTime.Parse("2019-07-03T10:11:09")
            };
            var store3 = new Store
            {
                Id = 3,
                P_BranchNo = 246,
                P_Name = "Store Kosong",
                P_Addr1 = "Lorong Kosong",
                CreatedOnUtc = DateTime.Parse("2019-07-05T01:39:19")
            };

            var dataSet = new List<Store> { store1, store2, store3 }.AsQueryable().BuildMockDbSet();
            _storeRepository.Setup(x => x.Table).Returns(dataSet?.Object);

            var engine = new Mock<Engine>();
            var serviceProvider = new TestGenericApiServiceProvider();

            _dbContext.Setup(x => x.Set<Store>()).Returns(_dbSet.Object);

            engine.Setup(x => x.ServiceProvider).Returns(serviceProvider);
            engine.Setup(x => x.ResolveUnregistered(typeof(Repository<Store>))).Returns(_storeRepository.Object);
            EngineContext.Replace(engine.Object);
        }

        [Test]
        public void Can_get_entities_based_on_their_dto()
        {
            var apiService = new GenericApiService<StoreDto>(_logger.Object, _dbContext.Object);

            var result = apiService.GetEntities();

            result.ShouldNotBeNull();
            result.Count.ShouldEqual(3);
        }

        [Test]
        public void Can_get_entity_by_id_based_on_its_dto()
        {
            var apiService = new GenericApiService<StoreDto>(_logger.Object, _dbContext.Object);

            //StoreDto will be use branch_no as its id
            var result = apiService.GetEntityById(135);

            result.Name.ShouldEqual("Store Power Root");
        }

        [Test]
        public void Can_get_entity_count_based_on_its_dto()
        {
            var apiService = new GenericApiService<StoreDto>(_logger.Object, _dbContext.Object);
            var count = apiService.GetEntityCount();

            count.ShouldEqual(3);
        }

        [Test]
        public void Can_search_entity_by_attributes_based_on_its_dto()
        {
            var apiService = new GenericApiService<StoreDto>(_logger.Object, _dbContext.Object);

            var no_query = "";
            var search1 = apiService.Search(no_query);

            search1.List.Count.ShouldEqual(3);

            var query_with_wrong_attribute = "P_BranchNo:246,P_Namee:Store Kosong";
            var search2 = apiService.Search(query_with_wrong_attribute);

            search2.List.Count.ShouldEqual(2);

            var query_single_valid = "P_Name:Store Kosong";
            var search3 = apiService.Search(query_single_valid);

            search3.List.Count.ShouldEqual(1);

            var query_multiple_all_valid = "P_BranchNo:246,P_Name:Store Kosong";
            var search4 = apiService.Search(query_multiple_all_valid);

            search4.List.Count.ShouldEqual(1);
            search4.List.First().Address1.ShouldEqual("Lorong Kosong");

            //2019-05-27T09:09:30.9814175
            var query_with_datetime = "createdonutc:-5";
            var search5 = apiService.Search(query_with_datetime);

            search5.List.Count.ShouldEqual(2);
            search5.List.Count(x => x.BranchNo == 246).ShouldEqual(2);

            var search6 = apiService.Search(query_with_datetime, count: true);
            search6.Count.ShouldEqual(2);

            var datetime_long = "createdonutc:-40";
            var datetimeLongSearch = apiService.Search(datetime_long, count:true);

            datetimeLongSearch.Count.ShouldEqual(2);

            var datetime_positive = "createdonutc:+5";
            var datetimePositiveSearch = apiService.Search(datetime_positive);

            datetimePositiveSearch.List.First().Id.ShouldEqual(1);

            var query_count_others = "p_branchno:246";

            var search7 = apiService.Search(query_count_others, limit:1);
            search7.List.Count.ShouldEqual(1);
            search7.Count.ShouldEqual(0);

            var search8 = apiService.Search(query_count_others, limit: 1, count: true);
            search8.Count.ShouldEqual(2);
            search8.List.ShouldBeNull();
        }
    }
}