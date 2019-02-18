using System;
using NUnit.Framework;
using StockManagementSystem.Core;
using Tests;

namespace Core.Tests
{
    public class CommonHelperEmailValidatorTests
    {
        [Test]
        public void Text_is_valid_email_address()
        {
            var email = "testuser@gmail.com";
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(true);
        }

        [Test]
        public void Text_is_valid_email_address_including_plus()
        {
            var email = "testuser+test@gmail.com";
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(true);
        }

        [Test]
        public void Text_is_null()
        {
            string email = null;
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(false);
        }

        [Test]
        public void Text_is_empty()
        {
            string email = String.Empty;
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(false);
        }

        [Test]
        public void Text_is_not_valid_email_address()
        {
            var email = "testuse";
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(false);
        }

        [Test]
        public void Text_should_not_hang()
        {
            var email = "thisisaverylongstringcodeplex.com";
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(false);
        }

        [Test]
        public void Text_contains_upper_cases()
        {
            var email = "TestUser@gmail.com";
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(true);
        }
    }
}