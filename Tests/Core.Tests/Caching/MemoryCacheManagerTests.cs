using System;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using StockManagementSystem.Core.Caching;
using Tests;

namespace Core.Tests.Caching
{
    [TestFixture]
    public class MemoryCacheManagerTests
    {
        [Test]
        public void Can_get_set_object_from_cache()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
            cacheManager.Set("key_1", 1, int.MaxValue);

            cacheManager.Get("key_1", () => 0).ShouldEqual(1);
        }

        [Test]
        public void Can_validate_cached_object()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
            cacheManager.Set("key_1", 1, int.MaxValue);
            cacheManager.Set("key_2", 2, int.MaxValue);

            cacheManager.IsSet("key_1").ShouldEqual(true);
            cacheManager.IsSet("key_3").ShouldEqual(false);
        }

        [Test]
        public void Can_clear_cache()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
            cacheManager.Set("key_1", 1, int.MaxValue);

            cacheManager.Clear();

            cacheManager.IsSet("key_1").ShouldEqual(false);
        }

        [Test]
        public void Can_lock_cache()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));

            var key = ".Task";
            var expiration = TimeSpan.FromMinutes(2);

            var actionCount = 0;
            var action = new Action(() =>
            {
                cacheManager.IsSet(key).ShouldBeTrue();

                cacheManager.PerformActionWithLock(key, expiration,
                    () => Assert.Fail("Action in progress")).ShouldBeFalse();

                if (++actionCount % 2 == 0)
                    throw new ApplicationException("Alternating actions fail");
            });

            cacheManager.PerformActionWithLock(key, expiration, action).ShouldBeTrue();
            actionCount.ShouldEqual(1);

            Assert.Throws<ApplicationException>(() => cacheManager.PerformActionWithLock(key, expiration, action));
            actionCount.ShouldEqual(2);

            cacheManager.PerformActionWithLock(key, expiration, action).ShouldBeTrue();
            actionCount.ShouldEqual(3);
        }
    }
}