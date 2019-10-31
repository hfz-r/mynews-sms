using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.DTOs.Stores;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Data;
using Tests;

namespace Api.Tests.ServicesTests.Generics
{
    [TestFixture]
    public class GenericApiServiceTests
    {
        private Mock<IDbContext> _dbContext;
        private Mock<IUnitOfWork> _worker;
        private Mock<IRepositoryAsync<Store>> _repository;

        [SetUp]
        public void SetUp()
        {
            _dbContext = new Mock<IDbContext>();
            _worker = new Mock<IUnitOfWork>();
            _repository = new Mock<IRepositoryAsync<Store>>();

            var store1 = new Store
            {
                Id = 1,
                P_BranchNo = 135,
                P_Name = "Store Power Root",
                //**synchronize with query_with_datetime (now() - 5) && datetime_long (now() - 40)
                CreatedOnUtc = DateTime.Parse("2019-10-26T06:14:30.9814175")
            };
            var store2 = new Store
            {
                Id = 2,
                P_BranchNo = 246,
                P_Name = "Store Kacip Fatimah Maa`don",
                //**synchronize with datetime_long (now() - 40)
                CreatedOnUtc = DateTime.Parse("2019-09-22T10:11:09") 
            };
            var store3 = new Store
            {
                Id = 3,
                P_BranchNo = 246,
                P_Name = "Store Kosong",
                P_Addr1 = "Lorong Kosong",
                //**synchronize with datetime_positive (now() + 5)
                CreatedOnUtc = DateTime.Parse("2019-11-04T01:39:19") 
            };

            var dataSet = new List<Store> {store1, store2, store3}.BuildMockDbSet();

            _dbContext.Setup(x => x.Set<Store>()).Returns(dataSet.Object);

            _repository.Setup(x => x.GetQueryableAsync(null, null, null, true)).ReturnsAsync(dataSet.Object);
            _worker.Setup(x => x.GetRepositoryAsync<Store>()).Returns(() => new RepositoryAsync<Store>(_dbContext.Object));
        }

        [Test]
        public void Can_get_entities_based_on_their_dto()
        {
            var apiService = new GenericApiService<StoreDto, Store>(_worker.Object);

            var result = apiService.GetAll(sortColumn: "branch_no", descending: true).GetAwaiter().GetResult();

            result.ShouldNotBeNull();
            result.Last().Name.ShouldEqual("Store Power Root");
            result.Count.ShouldEqual(3);
        }

        [Test]
        public void Can_get_entity_by_id_based_on_its_dto()
        {
            var apiService = new GenericApiService<StoreDto, Store>(_worker.Object);

            //storeId = branch_no
            var result = apiService.GetById(135).GetAwaiter().GetResult();

            result.Name.ShouldEqual("Store Power Root");
        }

        [Test]
        public void Can_get_entity_count_based_on_its_dto()
        {
            var apiService = new GenericApiService<StoreDto, Store>(_worker.Object);

            var count = apiService.Count().GetAwaiter().GetResult();

            count.ShouldEqual(3);
        }

        [Test]
        public void Can_search_entity_by_attributes_based_on_its_dto()
        {
            var apiService = new GenericApiService<StoreDto, Store>(_worker.Object);

            var query = "p_branchno:246";
            var sortColumn = "name";
            var search = apiService.Search(query, sortColumn: sortColumn).GetAwaiter().GetResult();

            search.List.Count.ShouldEqual(2);
            search.List.First().Id.ShouldEqual(2);

            var no_query = "";
            var search1 = apiService.Search(no_query).GetAwaiter().GetResult();

            search1.List.Count.ShouldEqual(3);

            var query_with_wrong_attribute = "branch_no:246,namee:Store Kosong";
            var search2 = apiService.Search(query_with_wrong_attribute).GetAwaiter().GetResult();

            search2.List.Count.ShouldEqual(2);

            var query_single_valid = "name:Store Kosong";
            var search3 = apiService.Search(query_single_valid).GetAwaiter().GetResult();

            search3.List.Count.ShouldEqual(1);

            var query_multiple_all_valid = "branch_no:246,name:Store Kosong";
            var search4 = apiService.Search(query_multiple_all_valid).GetAwaiter().GetResult();

            search4.List.Count.ShouldEqual(1);
            search4.List.First().Address1.ShouldEqual("Lorong Kosong");

            //for datetime test, see above notes marked with **synchronize
            var query_with_datetime = "created_on_utc:-5";
            var search5 = apiService.Search(query_with_datetime).GetAwaiter().GetResult();

            search5.List.Count.ShouldEqual(1);
            search5.List.Count(x => x.BranchNo == 135).ShouldEqual(1);

            var search6 = apiService.Search(query_with_datetime, count: true).GetAwaiter().GetResult();
            search6.Count.ShouldEqual(1);

            var datetime_long = "created_on_utc:-40";
            var datetimeLongSearch = apiService.Search(datetime_long, count: true).GetAwaiter().GetResult();

            datetimeLongSearch.Count.ShouldEqual(2);

            var datetime_positive = "created_on_utc:+5";
            var datetimePositiveSearch = apiService.Search(datetime_positive).GetAwaiter().GetResult();

            datetimePositiveSearch.List.First().Id.ShouldEqual(3);

            var query_count_others = "branch_no:246";

            var search7 = apiService.Search(query_count_others, sortColumn: "name", limit: 1).GetAwaiter().GetResult();
            search7.List.Count.ShouldEqual(1);
            search7.Count.ShouldEqual(0);

            var search8 = apiService.Search(query_count_others, limit: 1, count: true).GetAwaiter().GetResult();
            search8.Count.ShouldEqual(2);
            search8.List.ShouldBeNull();
        }
    }
}