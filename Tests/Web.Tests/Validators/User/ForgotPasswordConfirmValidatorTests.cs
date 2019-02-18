using FluentValidation.TestHelper;
using NUnit.Framework;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Validators.Users;

namespace Web.Tests.Validators.User
{
    [TestFixture]
    public class ForgotPasswordConfirmValidatorTests : BaseValidatorTests
    {
        private ForgotPasswordConfirmValidator _validator;
        private UserSettings _userSettings;

        [SetUp]
        public new void Setup()
        {
            _userSettings = new UserSettings();
            _validator = new ForgotPasswordConfirmValidator(_userSettings);
        }

        [Test]
        public void Should_have_error_when_newPassword_is_null_or_empty()
        {
            var model = new ForgotPasswordConfirmViewModel
            {
                NewPassword = null
            };

            //new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _validator.ShouldHaveValidationErrorFor(x => x.NewPassword, model);
            model.NewPassword = "";
            //new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _validator.ShouldHaveValidationErrorFor(x => x.NewPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_newPassword_is_specified()
        {
            var model = new ForgotPasswordConfirmViewModel
            {
                NewPassword = "new password"
            };

            //new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _validator.ShouldNotHaveValidationErrorFor(x => x.NewPassword, model);
        }

        [Test]
        public void Should_have_error_when_confirmNewPassword_is_null_or_empty()
        {
            var model = new ForgotPasswordConfirmViewModel
            {
                ConfirmNewPassword = null
            };

            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword, model);
            model.ConfirmNewPassword = "";
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_confirmNewPassword_is_specified()
        {
            var model = new ForgotPasswordConfirmViewModel
            {
                ConfirmNewPassword = "some password"
            };

            //wnew password should equal confirmation password
            model.NewPassword = model.ConfirmNewPassword;
            _validator.ShouldNotHaveValidationErrorFor(x => x.ConfirmNewPassword, model);
        }

        [Test]
        public void Should_have_error_when_newPassword_doesnot_equal_confirmationPassword()
        {
            var model = new ForgotPasswordConfirmViewModel
            {
                NewPassword = "some password",
                ConfirmNewPassword = "another password"
            };

            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_newPassword_equals_confirmationPassword()
        {
            var model = new ForgotPasswordConfirmViewModel
            {
                NewPassword = "some password",
                ConfirmNewPassword = "some password"
            };

            _validator.ShouldNotHaveValidationErrorFor(x => x.NewPassword, model);
        }
    }
}