using System;
using Core.Tests.Fakes;
using NUnit.Framework;
using StockManagementSystem.Core.Infrastructure;
using Tests;

namespace Core.Tests
{
    public abstract class TypeFindingBase : TestsBase
    {
        protected ITypeFinder typeFinder;

        protected abstract Type[] GetTypes();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            typeFinder = new FakeTypeFinder(typeof(TypeFindingBase).Assembly, GetTypes());
        }
    }
}