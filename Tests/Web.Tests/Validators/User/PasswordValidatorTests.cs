using FluentValidation.TestHelper;
using NUnit.Framework;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Validators.Users;
using StockManagementSystem.Web.Validators;

namespace Web.Tests.Validators.User
{
    [TestFixture]
    public class PasswordValidatorTests : BaseValidatorTests
    {
        private TestValidator _validator;
        private Person _person;
        private ChangePasswordValidator _changePasswordValidator;
        private ForgotPasswordConfirmValidator _forgotPasswordConfirmValidator;
        private RegisterValidator _registerValidator;
        private UserSettings _userSettings;

        [SetUp]
        public new void Setup()
        {
            _userSettings = new UserSettings
            {
                PasswordMinLength = 8,
                PasswordRequireUppercase = true,
                PasswordRequireLowercase = true,
                PasswordRequireDigit = true,
                PasswordRequireNonAlphanumeric = true
            };

            _changePasswordValidator = new ChangePasswordValidator(_userSettings);
            _registerValidator = new RegisterValidator(_userSettings);
            _forgotPasswordConfirmValidator = new ForgotPasswordConfirmValidator(_userSettings);

            _validator = new TestValidator();
            _person = new Person();
        }

        [Test]
        public void Is_valid_tests_lowercase()
        {
            var userSettings = new UserSettings
            {
                PasswordMinLength = 3,
                PasswordRequireLowercase = true
            };

            _validator.RuleFor(x => x.Password).IsPassword(userSettings);

            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "USER123");
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "user123");
        }

        [Test]
        public void Is_valid_tests_uppercase()
        {
            var userSettings = new UserSettings
            {
                PasswordMinLength = 3,
                PasswordRequireUppercase = true
            };

            _validator.RuleFor(x => x.Password).IsPassword(userSettings);

            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "user");
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "User");
        }

        [Test]
        public void Is_valid_tests_digit()
        {
            var userSettings = new UserSettings
            {
                PasswordMinLength = 3,
                PasswordRequireDigit = true
            };

            _validator.RuleFor(x => x.Password).IsPassword(userSettings);

            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "user");
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "User1");
        }


        [Test]
        public void Is_valid_tests_NonAlphanumeric()
        {
            var userSettings = new UserSettings
            {
                PasswordMinLength = 3,
                PasswordRequireNonAlphanumeric = true
            };

            _validator.RuleFor(x => x.Password).IsPassword(userSettings);

            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "user");
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "User#");

        }

        [Test]
        public void Is_valid_tests_all_rules()
        {
            //Example:  (?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$
            _validator.RuleFor(x => x.Password).IsPassword(_userSettings);

            //ShouldHaveValidationError
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "123");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "12345678");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "userproject");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "userProject");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "userproject123");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "userProject123");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "userproject123$");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "USERPROJECT123$");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "userProject123~");

            //ShouldNotHaveValidationError
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "userProject123$");
        }

        [Test]
        public void Should_validate_on_ChangePasswordModel_is_all_rule()
        {
            _changePasswordValidator = new ChangePasswordValidator(_userSettings);

            var model = new ChangePasswordModel
            {
                NewPassword = "1234"
            };

            //new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _changePasswordValidator.ShouldHaveValidationErrorFor(x => x.NewPassword, model);
            model.NewPassword = "userProject123$";
            //new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _changePasswordValidator.ShouldNotHaveValidationErrorFor(x => x.NewPassword, model);
        }

        [Test]
        public void Should_validate_on_PasswordRecoveryConfirmModel_is_all_rule()
        {
            _forgotPasswordConfirmValidator = new ForgotPasswordConfirmValidator(_userSettings);

            var model = new ForgotPasswordConfirmViewModel()
            {
                NewPassword = "1234"
            };
            //new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _forgotPasswordConfirmValidator.ShouldHaveValidationErrorFor(x => x.NewPassword, model);
            model.NewPassword = "userProject123$";
            //new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _forgotPasswordConfirmValidator.ShouldNotHaveValidationErrorFor(x => x.NewPassword, model);
        }

        [Test]
        public void Should_validate_on_RegisterModel_is_all_rule()
        {
            _registerValidator = new RegisterValidator(_userSettings);

            var model = new RegisterViewModel()
            {
                Password = "1234"
            };
            //password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _registerValidator.ShouldHaveValidationErrorFor(x => x.Password, model);
            model.Password = "userProject123$";
            //password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _registerValidator.ShouldNotHaveValidationErrorFor(x => x.Password, model);
        }
    }
}