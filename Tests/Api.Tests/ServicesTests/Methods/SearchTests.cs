using System.IO;
using System.Linq;
using Api.Tests.ServicesTests.Methods.DataSource;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Settings;
using Tests;

namespace Api.Tests.ServicesTests.Methods
{
    [TestFixture]
    public class SearchTests
    {
        private Mock<IRepository<ShelfLocation>> _repositoryMock;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IRepository<ShelfLocation>>();

            string path = Path.Combine(System.Environment.CurrentDirectory,
                "../../../ServicesTests/Methods/DataSource/shelflocation.xlsx");
            var file = new FileInfo(path);

            using (var pck = new ExcelPackage(file))
            {
                var workbook = pck.Workbook;
                var worksheet = workbook.Worksheets["sheet1"];

                var collection = worksheet.ConvertSheetToObjects<ShelfLocation>().ToList();
                _repositoryMock.Setup(x => x.Table).Returns(collection.BuildMockDbSet().Object);
            }
        }

        [Test]
        public void Can_search_multiple_queries()
        {
            var svc = new ShelfLocationApiService(_repositoryMock.Object);

            var query = "P_Gondola:8";
            var search = svc.Search(queryParams: query);

            search.List.Count.ShouldEqual(5);

            var query1 = "P_Gondola:8,P_Row:8";
            var search1 = svc.Search(queryParams: query1);

            search1.List.Count.ShouldEqual(2);

            var query2 = "P_Gondola:8,P_Row:5,P_Face:2";
            var search2 = svc.Search(queryParams: query2);

            search2.List.Count.ShouldEqual(1);
            search2.List.First().Id.ShouldEqual(509);
        }
    }
}