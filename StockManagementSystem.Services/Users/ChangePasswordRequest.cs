using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Users
{
    /// <summary>
    /// Change password request
    /// </summary>
    public class ChangePasswordRequest
    {
        public string Email { get; set; }

        public bool ValidateRequest { get; set; }

        public PasswordFormat NewPasswordFormat { get; set; }

        public string NewPassword { get; set; }

        public string OldPassword { get; set; }

        /// <summary>
        /// Hashed password format (e.g. SHA1, SHA512)
        /// </summary>
        public string HashedPasswordFormat { get; set; }

        public ChangePasswordRequest(string email, bool validateRequest, PasswordFormat newPasswordFormat, string newPassword, 
            string oldPassword = "",  string hashedPasswordFormat = null)
        {
            this.Email = email;
            this.ValidateRequest = validateRequest;
            this.NewPasswordFormat = newPasswordFormat;
            this.NewPassword = newPassword;
            this.OldPassword = oldPassword;
            this.HashedPasswordFormat = hashedPasswordFormat;
        }
    }
}