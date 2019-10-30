using Data.Tests.TestDatabase;
using Data.Tests.TestFixtures;
using NUnit.Framework;
using StockManagementSystem.Data;

namespace Data.Tests
{
    [TestFixture]
    public class RepositoryAddTests
    {
        private InMemoryTestFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new InMemoryTestFixture();
        }

        [Test]
        public void ShouldAddNewCategory()
        {
            using (var uow = new UnitOfWork<TestDbContext>(_fixture.Context))
            {
                var repo = uow.GetRepositoryAsync<TestCategory>();
                var newCategory = new TestCategory {Id = 666, Name = "Unit Test Product"};

                repo.AddAsync(newCategory).GetAwaiter().GetResult();
                uow.SaveChangesAsync().GetAwaiter().GetResult();

                Assert.AreEqual(666, newCategory.Id);
            }
        }

        [Test]
        public void ShouldAddNewProduct()
        {
            using (var uow = new UnitOfWork<TestDbContext>(_fixture.Context))
            {
                var repo = uow.GetRepositoryAsync<TestProduct>();
                var newProduct = new TestProduct
                {
                    Id = 777,
                    Name = "Test Product",
                    Category = new TestCategory {Id = 666, Name = "Unit Test Product"}
                };

                repo.AddAsync(newProduct).GetAwaiter().GetResult();
                uow.SaveChangesAsync().GetAwaiter().GetResult();

                Assert.AreEqual(777, newProduct.Id);
            }
        }
    }
}