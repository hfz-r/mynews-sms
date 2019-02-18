using FluentValidation.TestHelper;
using NUnit.Framework;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Validators.Users;

namespace Web.Tests.Validators.User
{
    [TestFixture]
    public class ForgotPasswordValidatorTests : BaseValidatorTests
    {
        private ForgotPasswordValidator _validator;

        [SetUp]
        public new void Setup()
        {
            _validator = new ForgotPasswordValidator();
        }

        [Test]
        public void Should_have_error_when_email_is_null_or_empty()
        {
            var model = new ForgotPasswordViewModel
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
            var model = new ForgotPasswordViewModel
            {
                Email = "userexample.com"
            };

            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_not_have_error_when_email_is_correct_format()
        {
            var model = new ForgotPasswordViewModel
            {
                Email = "user@example.com"
            };

            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }
    }
}