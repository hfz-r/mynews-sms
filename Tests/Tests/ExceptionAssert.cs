using System;
using System.Reflection;
using NUnit.Framework;

namespace Tests
{
    /// <summary>
    /// Contains exception assertions used for unit testing.
    /// </summary>
    public class ExceptionAssert
    {
        public delegate void ExceptionDelegate();

        /// <summary>
        /// Executes a method and asserts that the specified exception is thrown.
        /// </summary>
        /// <param name="exceptionType">The type of exception to expect.</param>
        /// <param name="method">The method to execute.</param>
        /// <returns>The thrown exception.</returns>
        public static Exception Throws(Type exceptionType, ExceptionDelegate method)
        {
            try
            {
                method.Invoke();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(exceptionType, ex.GetType());
                return ex;
            }
            Assert.Fail("Expected exception '" + exceptionType.FullName + "' wasn't thrown.");

            return null;
        }

        /// <summary>
        /// Executes a method and asserts that the specified exception is thrown.
        /// </summary>
        /// <typeparam name="T">The type of exception to expect.</typeparam>
        /// <param name="method">The method to execute.</param>
        /// <returns>The thrown exception.</returns>
        public static T Throws<T>(ExceptionDelegate method) where T : Exception
        {
            try
            {
                method.Invoke();
            }
            catch (TargetInvocationException ex)
            {
                Assert.That(ex.InnerException, Is.TypeOf(typeof(T)));
            }
            catch (T ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected exception '" + typeof(T).FullName + "' but got exception '" + ex.GetType() + "'.");
                return null;
            }
            Assert.Fail("Expected exception '" + typeof(T).FullName + "' wasn't thrown.");

            return null;
        }

        /// <summary>
        /// Executes a method and asserts that the specified exception is thrown.
        /// </summary>
        /// <typeparam name="T">The type of exception to expect.</typeparam>
        /// <param name="method">The method to execute.</param>
        /// <returns>The thrown exception.</returns>
        public static void InnerException<T>(ExceptionDelegate method)  where T : Exception
        {
            try
            {
                method.Invoke();
            }
            catch (Exception ex)
            {
                TypeAssert.AreEqual(typeof(T), ex.InnerException);

                return;
            }
            Assert.Fail("Expected exception '" + typeof(T).FullName + "' wasn't thrown.");
        }
    }

    [TestFixture]
    public class ExceptionAssertTests
    {
        [Test]
        public void PassesOnExceptionThrown()
        {
            ExceptionAssert.Throws(typeof(ArgumentException), () => throw new ArgumentException("catch me"));
        }

        [Test]
        public void ReturnsTheException()
        {
            var ex = ExceptionAssert.Throws(typeof(ArgumentException), () => throw new ArgumentException("return me"));
            Assert.AreEqual("return me", ex.Message);
        }

        [Test]
        public void PassesOnExceptionThrown_generic()
        {
            ExceptionAssert.Throws<ArgumentException>(
                () => throw new ArgumentException("catch me"));
        }

        [Test]
        public void ReturnsTheException_generic()
        {
            var ex = ExceptionAssert.Throws<ArgumentException>(
                () => throw new ArgumentException("return me"));

            Assert.AreEqual("return me", ex.Message);
        }
    }
}