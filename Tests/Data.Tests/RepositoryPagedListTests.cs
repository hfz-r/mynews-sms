using System.Linq;
using Data.Tests.TestDatabase;
using Data.Tests.TestFixtures;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using StockManagementSystem.Data;
using StockManagementSystem.Data.Paging;

namespace Data.Tests
{
    [TestFixture]
    public class RepositoryPagedListTests
    {
        private InMemoryTestFixture _testFixture;

        [SetUp]
        public void SetUp()
        {
            _testFixture = new InMemoryTestFixture();
        }

        [Test]
        public void GetPagedListIncludesTest()
        {
            using (var uow = new UnitOfWork<TestDbContext>(_testFixture.Context))
            {
                var categories = uow.GetRepositoryAsync<TestCategory>()
                    .GetPagedListAsync(include: src => src.Include(p => p.Products).ThenInclude(p => p.Category), size: 5)
                    .GetAwaiter().GetResult();

                Assert.IsAssignableFrom<Paginate<TestCategory>>(categories);

                Assert.AreEqual(20, categories.Count);
                Assert.AreEqual(4, categories.Pages);
                Assert.AreEqual(5, categories.Items.Count);
            }
        }

        [Test]
        public void GetProductPagedListUsingPredicateTest()
        {
            using (var uow = new UnitOfWork<TestDbContext>(_testFixture.Context))
            {
                var repo = uow.GetRepositoryAsync<TestProduct>();

                var productList = repo.GetPagedListAsync(predicate: src => src.CategoryId == 1).GetAwaiter().GetResult();

                Assert.AreEqual(5, productList.Count);
            }
        }

        [Test]
        public void GetGenericPagedListTest()
        {
            using (var uow = new UnitOfWork<TestDbContext>(_testFixture.Context))
            {
                var repo = uow.GetRepositoryAsync<TestProduct>();

                var category = repo.GetPagedListAsync(predicate: src => src.InStock == true, selector: src => src.Category)
                    .GetAwaiter().GetResult();

                Assert.IsNotNull(category);
                Assert.IsTrue(category.Count > 5);
            }
        }

        [Test]
        public void ShouldReturnAsyncInterface()
        {
            using (var uow = new UnitOfWork<TestDbContext>(_testFixture.Context))
            {
                var repo = uow.GetRepositoryAsync<TestProduct>();

                Assert.IsInstanceOf<IRepositoryAsync<TestProduct>>(repo);
            }
        }

        [Test]
        public void ShouldReadFromProducts()
        {
            using (var uow = new UnitOfWork<TestDbContext>(_testFixture.Context))
            {
                var repo = uow.GetRepositoryAsync<TestProduct>();

                var products = repo.GetPagedListAsync().GetAwaiter().GetResult().Items;

                Assert.AreEqual(20, products.Count);
            }
        }
    }
}