using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation.Validators;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Web.Validators
{
    /// <summary>
    /// Username validator
    /// </summary>
    public class UsernamePropertyValidator : PropertyValidator
    {
        private readonly UserSettings _userSettings;

        public UsernamePropertyValidator(UserSettings userSettings) : base("Username is not valid")
        {
            _userSettings = userSettings;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            return IsValid(context.PropertyValue as string, _userSettings);
        }

        public static bool IsValid(string username, UserSettings userSettings)
        {
            if (!userSettings.UsernameValidationEnabled || string.IsNullOrEmpty(userSettings.UsernameValidationRule))
                return true;

            if (string.IsNullOrEmpty(username))
                return false;

            return userSettings.UsernameValidationUseRegex
                ? Regex.IsMatch(username, userSettings.UsernameValidationRule,
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                : username.All(l => userSettings.UsernameValidationRule.Contains(l));
        }
    }
}