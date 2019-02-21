using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Users
{
    public class UserRegistrationRequest
    {
        public UserRegistrationRequest(User user, string email, string username, string password,
            PasswordFormat passwordFormat, int tenantId, bool isApproved = true)
        {
            this.User = user;
            this.Email = email;
            this.Username = username;
            this.Password = password;
            this.PasswordFormat = passwordFormat;
            this.IsApproved = isApproved;
            this.TenantId = tenantId;
        }

        public User User { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public PasswordFormat PasswordFormat { get; set; }

        public int TenantId { get; set; }

        public bool IsApproved { get; set; }
    }
}