using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Stores;
using Tests;

namespace Api.Tests.ServicesTests.PushNotifications
{
    [TestFixture]
    public class PushNotificationApiServiceTests
    {
        private Mock<IRepository<PushNotification>> _pushNotificationRepository;
        private Mock<IRepository<PushNotificationStore>> _pushNotificationStoreRepository;

        [SetUp]
        public void SetUp()
        {
            _pushNotificationRepository = new Mock<IRepository<PushNotification>>();
            _pushNotificationStoreRepository = new Mock<IRepository<PushNotificationStore>>();

            #region Dataset

            var pn1 = new PushNotification
            {
                Id = 1,
                Title = "Swagger",
                Desc = "Push notification swagger",
                StockTakeNo = 123,
                StartTime = DateTime.Parse("2019-07-10T09:09:30.9814175"),
                EndTime = DateTime.Parse("2019-07-15"),
                CreatedOnUtc = DateTime.Now,
            };
            var pn2 = new PushNotification
            {
                Id = 2,
                Title = "Hipster",
                Desc = "Push notification hipster",
                StockTakeNo = 666,
                StartTime = DateTime.Parse("2019-07-09T09:09:30.9814175"),
                EndTime = DateTime.Parse("2019-07-10"),
                CreatedOnUtc = DateTime.Now,
            };
            var pn3 = new PushNotification
            {
                Id = 3,
                Title = "Big mouth",
                Desc = "Push notification big mouth",
                StockTakeNo = 666,
                StartTime = DateTime.Parse("2019-07-08T09:09:30.9814175"),
                EndTime = DateTime.Parse("2019-07-20"),
                CreatedOnUtc = DateTime.Now,
            };
            var store1 = new Store
            {
                P_BranchNo = 135,
                P_Name = "Store Power Root",
                CreatedOnUtc = DateTime.Parse("2019-07-10T09:09:30.9814175")
            };
            var store2 = new Store
            {
                P_BranchNo = 246,
                P_Name = "Store Kacip Fatimah Maa`don",
                CreatedOnUtc = DateTime.Parse("2019-07-03T10:11:09")
            };
            var pns1 = new PushNotificationStore
            {
                Id = 1,
                PushNotificationId = pn1.Id,
                PushNotification = pn1,
                StoreId = store2.P_BranchNo,
                Store = store2
            };
            var pns2 = new PushNotificationStore
            {
                Id = 2,
                PushNotificationId = pn1.Id,
                PushNotification = pn1,
                StoreId = store1.P_BranchNo,
                Store = store1
            };
            var pns3 = new PushNotificationStore
            {
                Id = 3,
                PushNotificationId = pn2.Id,
                PushNotification = pn2,
                StoreId = store2.P_BranchNo,
                Store = store2
            };
            var pns4 = new PushNotificationStore
            {
                Id = 4,
                PushNotificationId = pn3.Id,
                PushNotification = pn3,
                StoreId = store2.P_BranchNo,
                Store = store2
            };

            pn1.PushNotificationStores.Add(pns1);
            pn1.PushNotificationStores.Add(pns2);
            pn2.PushNotificationStores.Add(pns3);
            pn3.PushNotificationStores.Add(pns4);

            #endregion

            var pnData = new List<PushNotification> {pn1, pn2, pn3}.BuildMockDbSet();
            _pushNotificationRepository.Setup(x => x.Table).Returns(pnData.Object);

            var pnsData = new List<PushNotificationStore> {pns1, pns2, pns3, pns4}.BuildMockDbSet();
            _pushNotificationStoreRepository.Setup(x => x.Table).Returns(pnsData.Object);
        }

        [Test]
        public void can_search_push_notification_with_simple_query()
        {
            var services = new PushNotificationApiService(_pushNotificationRepository.Object, _pushNotificationStoreRepository.Object);

            //test1: no query given
            var search1 = services.Search();
            search1.List.Count.ShouldEqual(3);

            //test2: query with wrong attribute
            var query2 = "StockTakeNo:666,Description:Push notification swagger";
            var search2 = services.Search(queryParams: query2);
            search2.List.Count.ShouldEqual(2);

            //test3: one valid query
            var query3 = "stocktakeno:123";
            var search3 = services.Search(queryParams: query3);
            search3.List.First().Id.ShouldEqual(1);

            //test4: multiple valid queries
            var query4 = "stocktakeno:666,desc:Push notification big mouth";
            var search4 = services.Search(queryParams: query4);
            search4.List.First().Title.ShouldEqual("Big mouth");
        }

        [Test]
        public void can_search_push_notification_by_complex_query()
        {
            var services = new PushNotificationApiService(_pushNotificationRepository.Object, _pushNotificationStoreRepository.Object);

            //test1: include storeid
            var search1 = services.Search(storeId: 135);
            search1.List.Count.ShouldEqual(1);

            //test2: multiple valid queries 
            var query2 = "stocktakeno:666";
            var search2 = services.Search(storeId: 246, queryParams: query2);
            search2.List.Count.ShouldEqual(2);
            search2.List.First().Stores.First().Name.ShouldEqual("Store Kacip Fatimah Maa`don");

            //test3: invalid storeid with multiple queries
            var query3 = "title:Hipster,stocktakeno:123";
            var search3 = services.Search(storeId: 1, queryParams: query3);
            search3.List.Count.ShouldEqual(0);

            //test4: all valid
            var query4 = "stocktakeno:666,title:Hipster";
            var search4 = services.Search(storeId: 246, queryParams: query4);
            search4.List.Count.ShouldEqual(1);
            search4.List.First().Id.ShouldEqual(2);
        }

        [Test]
        public void can_search_push_notification_with_count_parameters()
        {
            var services = new PushNotificationApiService(_pushNotificationRepository.Object, _pushNotificationStoreRepository.Object);

            //test1: basic query
            var query1 = "stocktakeno:666";
            var search1 = services.Search(queryParams: query1, count: true);
            search1.Count.ShouldEqual(2);

            //test2: basic multiple queries
            var query2 = "desc:notification,stocktakeno:666";
            var search2 = services.Search(queryParams: query2, count: true);
            search2.Count.ShouldEqual(2);

            //test3: complex query
            var query3 = "desc:notification,stocktakeno:123";
            var search3 = services.Search(storeId: 246, queryParams: query3, count: true);
            search3.Count.ShouldEqual(1);

            //test4: invalid storeid
            var search4 = services.Search(storeId: 1, queryParams: query3, count: true);
            search4.Count.ShouldEqual(0);
        }

        [Test]
        public void can_search_push_notification_with_datetime_type()
        {
            var services = new PushNotificationApiService(_pushNotificationRepository.Object, _pushNotificationStoreRepository.Object);

            //test1: single query date
            var query1 = "starttime:-1";
            var search1 = services.Search(queryParams: query1);
            search1.List.Count.ShouldEqual(1);

            //test2: date range
            var query2 = "starttime:-2,endtime:+20";
            var search2 = services.Search(queryParams: query2);
            search2.List.First().Title.ShouldEqual("Swagger");

            //test3: date with storeid
            var query3 = "starttime:-3,stocktakeno:666,endtime:+30";
            var search3 = services.Search(storeId: 246, queryParams: query3);
            search3.List.First().Id.Equals(3);
        }

        [Test]
        public void can_search_push_notification_with_multiple_parameters()
        {
            var services = new PushNotificationApiService(_pushNotificationRepository.Object, _pushNotificationStoreRepository.Object);

            //test1: blend1
            var query1 = "stocktakeno:123,starttime:-2";
            var search1 = services.Search(storeId: 135, queryParams: query1, count: true);
            search1.Count.ShouldEqual(1);

            //test2: blend2
            var query2 = "starttime:-3";
            var search2 = services.Search(storeId: 246, queryParams: query2, sortColumn: "title");
            search2.List.Count.ShouldEqual(3);
            search2.List.First().Id.ShouldEqual(3);

            //test3: limit not applicable to count
            var search3 = services.Search(queryParams: query2, limit: 1, count: true);
            search3.Count.ShouldEqual(3);
        }
    }
}