using FluentValidation;

namespace StockManagementSystem.Web.Validators
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, decimal> IsDecimal<T>(this IRuleBuilder<T, decimal> ruleBuilder,
            decimal maxValue)
        {
            return ruleBuilder.SetValidator(new DecimalPropertyValidator(maxValue));
        }

        //TODO: user-password setting maybe?
        public static IRuleBuilder<T, string> IsPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var regExp = "^";
            //regExp += "(?=.*?[A-Z])"; //RequireUppercase
            //regExp += "(?=.*?[a-z])"; //RequireLowercase
            //regExp += "(?=.*?[0-9])"; //RequireDigit
            //regExp += "(?=.*?[#?!@$%^&*-])"; //RequireNonAlphanumeric
            regExp += ".{{8,}}$"; //MinLength

            var message = "<p>must meet the following rules: </p><ul>" +
                          "<li>must have at least 8 characters</li>" +
                          //"<li>must have at least one uppercase</li>" +
                          //"<li>must have at least one lowercase</li>" +
                          //"<li>must have at least one digit</li>" +
                          //"<li>must have at least one special character (e.g. #?!@$%^&*-)</li>" +
                          "</ul>";

            var options = ruleBuilder
                .NotEmpty().WithMessage("Password is required.");
                //.Matches(regExp).WithMessage(message);

            return options;
        }
    }
}