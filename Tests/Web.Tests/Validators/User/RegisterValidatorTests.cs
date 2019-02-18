using FluentValidation.TestHelper;
using NUnit.Framework;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Validators.Users;

namespace Web.Tests.Validators.User
{
    [TestFixture]
    public class RegisterValidatorTests : BaseValidatorTests
    {
        private RegisterValidator _validator;
        private UserSettings _userSettings;

        [SetUp]
        public new void Setup()
        {
            _userSettings = new UserSettings();
            _validator = new RegisterValidator(_userSettings);
        }

        [Test]
        public void Should_have_error_when_email_is_null_or_empty()
        {
            var model = new RegisterViewModel {Email = null};
            _validator.ShouldHaveValidationErrorFor(vm => vm.Email, model);

            model.Email = "";
            _validator.ShouldHaveValidationErrorFor(vm => vm.Email, model);
        }

        [Test]
        public void Should_have_error_when_email_is_wrong_format()
        {
            var model = new RegisterViewModel {Email = "userexample.com"};
            _validator.ShouldHaveValidationErrorFor(vm => vm.Email, model);
        }

        [Test]
        public void Should_not_have_error_when_email_is_correct_format()
        {
            var model = new RegisterViewModel {Email = "user@example.com"};
            _validator.ShouldNotHaveValidationErrorFor(vm => vm.Email, model);
        }

        [Test]
        public void Should_have_error_when_firstName_is_null_or_empty()
        {
            var model = new RegisterViewModel {FirstName = null};
            _validator.ShouldHaveValidationErrorFor(vm => vm.FirstName, model);

            model.FirstName = "";
            _validator.ShouldHaveValidationErrorFor(vm => vm.FirstName, model);
        }

        [Test]
        public void Should_not_have_error_when_firstName_is_specified()
        {
            var model = new RegisterViewModel {FirstName = "John"};
            _validator.ShouldNotHaveValidationErrorFor(vm => vm.FirstName, model);
        }

        [Test]
        public void Should_have_error_when_lastName_is_null_or_empty()
        {
            var model = new RegisterViewModel {LastName = null};
            _validator.ShouldHaveValidationErrorFor(vm => vm.LastName, model);

            model.LastName = "";
            _validator.ShouldHaveValidationErrorFor(vm => vm.LastName, model);
        }

        [Test]
        public void Should_not_have_error_when_lastName_is_specified()
        {
            var model = new RegisterViewModel {LastName = "Smith"};
            _validator.ShouldNotHaveValidationErrorFor(vm => vm.LastName, model);
        }

        [Test]
        public void Should_have_error_when_password_is_null_or_empty()
        {
            var model = new RegisterViewModel {Password = null};
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _validator.ShouldHaveValidationErrorFor(vm => vm.Password, model);

            model.Password = "";
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _validator.ShouldHaveValidationErrorFor(vm => vm.Password, model);
        }

        [Test]
        public void Should_not_have_error_when_password_is_specified()
        {
            var model = new RegisterViewModel
            {
                Password = "password"
            };
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _validator.ShouldNotHaveValidationErrorFor(vm => vm.Password, model);
        }

        [Test]
        public void Should_have_error_when_confirmPassword_is_null_or_empty()
        {
            var model = new RegisterViewModel {ConfirmPassword = null};
            _validator.ShouldHaveValidationErrorFor(vm => vm.ConfirmPassword, model);

            model.ConfirmPassword = "";
            _validator.ShouldHaveValidationErrorFor(vm => vm.ConfirmPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_confirmPassword_is_specified()
        {
            var model = new RegisterViewModel {ConfirmPassword = "some password"};
            //we know that new password should equal confirmation password
            model.Password = model.ConfirmPassword;
            _validator.ShouldNotHaveValidationErrorFor(vm => vm.ConfirmPassword, model);
        }

        [Test]
        public void Should_have_error_when_password_doesnot_equal_confirmationPassword()
        {
            var model = new RegisterViewModel
            {
                Password = "some password",
                ConfirmPassword = "another password"
            };
            _validator.ShouldHaveValidationErrorFor(vm => vm.ConfirmPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_password_equals_confirmationPassword()
        {
            var model = new RegisterViewModel
            {
                Password = "some password",
                ConfirmPassword = "some password"
            };
            _validator.ShouldNotHaveValidationErrorFor(vm => vm.Password, model);
        }
    }
}