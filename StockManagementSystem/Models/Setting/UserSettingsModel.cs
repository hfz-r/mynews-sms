using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class UserSettingsModel : BaseModel, ISettingsModel
    {
        public int ActiveTenantScopeConfiguration { get; set; }

        [Display(Name = "'Usernames' enabled")]
        public bool UsernamesEnabled { get; set; }

        [Display(Name = "Allow users to change their usernames")]
        public bool AllowUsersToChangeUsernames { get; set; }

        [Display(Name = "Allow users to check the availability of usernames")]
        public bool CheckUsernameAvailabilityEnabled { get; set; }

        [Display(Name = "Username validation is enabled")]
        public bool UsernameValidationEnabled { get; set; }

        [Display(Name = "Use regex for username validation")]
        public bool UsernameValidationUseRegex { get; set; }

        [Display(Name = "Username validation rule")]
        public string UsernameValidationRule { get; set; }

        [Display(Name = "Password minimum length")]
        public int PasswordMinLength { get; set; }

        [Display(Name = "Password must have at least one lowercase")]
        public bool PasswordRequireLowercase { get; set; }

        [Display(Name = "Password must have at least one uppercase")]
        public bool PasswordRequireUppercase { get; set; }

        [Display(Name = "Password must have at least one non alphanumeric character")]
        public bool PasswordRequireNonAlphanumeric { get; set; }

        [Display(Name = "Password must have at least one digit")]
        public bool PasswordRequireDigit { get; set; }

        [Display(Name = "Unduplicated passwords number")]
        public int UnduplicatedPasswordsNumber { get; set; }

        [Display(Name = "Password recovery link. Days valid")]
        public int PasswordRecoveryLinkDaysValid { get; set; }

        [Display(Name = "Default password format")]
        public int DefaultPasswordFormat { get; set; }

        [Display(Name = "Password lifetime")]
        public int PasswordLifetime { get; set; }

        [Display(Name = "Maximum login failures")]
        public int FailedPasswordAllowedAttempts { get; set; }

        [Display(Name = "Lockout time (login failures)")]
        public int FailedPasswordLockoutMinutes { get; set; }

        [Display(Name = "Allow users to upload avatars")]
        public bool AllowUsersToUploadAvatars { get; set; }

        [Display(Name = "Default avatar enabled")]
        public bool DefaultAvatarEnabled { get; set; }

        [Display(Name = "Store last visited page")]
        public bool StoreLastVisitedPage { get; set; }

        [Display(Name = "Store IP addresses")]
        public bool StoreIpAddresses { get; set; }

        [Display(Name = "'Gender' enabled")]
        public bool GenderEnabled { get; set; }

        [Display(Name = "'Date of Birth' enabled")]
        public bool DateOfBirthEnabled { get; set; }

        [Display(Name = "'Date of Birth' required")]
        public bool DateOfBirthRequired { get; set; }

        [Display(Name = "User minimum age")]
        [UIHint("Int32Nullable")]
        public int? DateOfBirthMinimumAge { get; set; }

        [Display(Name = "'Phone number' enabled")]
        public bool PhoneEnabled { get; set; }

        [Display(Name = "'Phone number' required")]
        public bool PhoneRequired { get; set; }
    }
}