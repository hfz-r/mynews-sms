using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Bogus.Extensions;
using Data.Tests.TestDatabase;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;

namespace Data.Tests.TestFixtures
{
    public class InMemoryTestFixture : IDisposable
    {
        public TestDbContext Context => InMemoryContext();

        public List<TestCategory> Categories { get; set; }
        public List<TestProduct> Products { get; set; }

        private TestDbContext InMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            InitFakerData();

            var context = new TestDbContext(options);

            context.TestCategories.AddRange(Categories);
            context.TestProducts.AddRange(Products);

            context.SaveChanges();

            return context;
        }

        private void InitFakerData()
        {
            var faker = new Faker();

            var categoryIds = 1;
            var categories = Builder<TestCategory>.CreateListOfSize(20)
                .All()
                    .With(c => c.Id = categoryIds++)
                    .With(c => c.Name = faker.Name.FirstName())
                .Build();

            Categories = categories.ToList();

            var productIds = 1;
            var products = Builder<TestProduct>.CreateListOfSize(20)
                .All()
                    .With(p => p.Id = productIds++)
                    .With(p => p.Name = faker.Commerce.ProductName())
                .TheFirst(5)
                    .With(p => p.CategoryId = 1)
                    .With(p => p.InStock = true)
                .TheNext(5)
                    .With(p => p.InStock = false)
                    .With(p => p.Stock = faker.Random.Number(10, 20))
                .Build();

            Products = products.ToList();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}