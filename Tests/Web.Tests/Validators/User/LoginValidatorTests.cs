using FluentValidation.TestHelper;
using NUnit.Framework;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Validators.Users;

namespace Web.Tests.Validators.User
{
    [TestFixture]
    public class LoginValidatorTests : BaseValidatorTests
    {
        private LoginValidator _validator;
        private UserSettings _userSettings;

        [SetUp]
        public new void Setup()
        {
            _userSettings = new UserSettings();
            _validator = new LoginValidator(_userSettings);
        }

        [Test]
        public void Should_have_error_when_email_is_null_or_empty()
        {
            var model = new LoginViewModel
            {
                Email = null
            };

            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
            model.Email = "";
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_have_error_when_email_is_wrong_format()
        {
            var model = new LoginViewModel
            {
                Email = "userexample.com"
            };

            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_not_have_error_when_email_is_correct_format()
        {
            var model = new LoginViewModel
            {
                Email = "user@example.com"
            };

            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_not_have_error_when_email_is_null_but_usernames_are_enabled()
        {
            _userSettings = new UserSettings
            {
                UsernamesEnabled = true
            };

            _validator = new LoginValidator(_userSettings);

            var model = new LoginViewModel {Email = null};
            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }
    }
}