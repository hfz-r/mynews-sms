using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Models.Install;
using StockManagementSystem.Validators.Install;

namespace Web.Tests.Validators.Install
{
    [TestFixture]
    public class InstallValidatorTests : BaseValidatorTests
    {
        private InstallValidator _validator;

        [SetUp]
        public new void Setup()
        {
            _validator = new InstallValidator();
        }

        [Test]
        public void Should_have_error_when_adminEmail_is_null_or_empty()
        {
            var model = new InstallModel
            {
                AdminEmail = null
            };

            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
            model.AdminEmail = "";
            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void Should_have_error_when_adminEmail_is_wrong_format()
        {
            var model = new InstallModel
            {
                AdminEmail = "adminexample.com"
            };

            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void Should_not_have_error_when_adminEmail_is_correct_format()
        {
            var model = new InstallModel
            {
                AdminEmail = "admin@example.com"
            };

            _validator.ShouldNotHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void Should_have_error_when_password_is_null_or_empty()
        {
            var model = new InstallModel
            {
                AdminPassword = null
            };

            //password should equal confirmation password
            model.ConfirmPassword = model.AdminPassword;
            _validator.ShouldHaveValidationErrorFor(x => x.AdminPassword, model);
            model.AdminPassword = "";
            //password should equal confirmation password
            model.ConfirmPassword = model.AdminPassword;
            _validator.ShouldHaveValidationErrorFor(x => x.AdminPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_password_is_specified()
        {
            var model = new InstallModel
            {
                AdminPassword = "password"
            };

            //password should equal confirmation password
            model.ConfirmPassword = model.AdminPassword;
            _validator.ShouldNotHaveValidationErrorFor(x => x.AdminPassword, model);
        }

        [Test]
        public void Should_have_error_when_confirmPassword_is_null_or_empty()
        {
            var model = new InstallModel
            {
                ConfirmPassword = null
            };

            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
            model.ConfirmPassword = "";
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_confirmPassword_is_specified()
        {
            var model = new InstallModel
            {
                ConfirmPassword = "some password"
            };

            _validator.ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void Should_have_error_when_password_doesnot_equal_confirmationPassword()
        {
            var model = new InstallModel
            {
                AdminPassword = "some password",
                ConfirmPassword = "another password"
            };

            _validator.ShouldHaveValidationErrorFor(x => x.AdminPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_password_equals_confirmationPassword()
        {
            var model = new InstallModel
            {
                AdminPassword = "some password",
                ConfirmPassword = "some password"
            };

            _validator.ShouldNotHaveValidationErrorFor(x => x.AdminPassword, model);
        }
    }
}