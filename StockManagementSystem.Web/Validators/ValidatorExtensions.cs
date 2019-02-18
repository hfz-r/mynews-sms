using FluentValidation;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Web.Validators
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, decimal> IsDecimal<T>(this IRuleBuilder<T, decimal> ruleBuilder,
            decimal maxValue)
        {
            return ruleBuilder.SetValidator(new DecimalPropertyValidator(maxValue));
        }

        public static IRuleBuilder<T, string> IsPassword<T>(this IRuleBuilder<T, string> ruleBuilder, UserSettings userSettings)
        {
            var regExp = "^";
            //Passwords must be at least X characters and contain the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*-)
            regExp += userSettings.PasswordRequireUppercase ? "(?=.*?[A-Z])" : "";
            regExp += userSettings.PasswordRequireLowercase ? "(?=.*?[a-z])" : "";
            regExp += userSettings.PasswordRequireDigit ? "(?=.*?[0-9])" : "";
            regExp += userSettings.PasswordRequireNonAlphanumeric ? "(?=.*?[#?!@$%^&*-])" : "";
            regExp += $".{{{userSettings.PasswordMinLength},}}$";

            var message = string.Format("<p>must meet the following rules: </p><ul>{0}{1}{2}{3}{4}</ul>", 
                string.Format("<li>must have at least {0} characters</li>", userSettings.PasswordMinLength),
                userSettings.PasswordRequireUppercase ? "<li>must have at least one uppercase</li>" : "",
                userSettings.PasswordRequireLowercase ? "<li>must have at least one lowercase</li>" : "",
                userSettings.PasswordRequireDigit ? "<li>must have at least one digit</li>" : "",
                userSettings.PasswordRequireNonAlphanumeric ? "<li>must have at least one special character (e.g. #?!@$%^&*-)</li>" : "");

            var options = ruleBuilder
                .NotEmpty().WithMessage("Password is required.");
                //.Matches(regExp).WithMessage(message);

            return options;
        }
    }
}