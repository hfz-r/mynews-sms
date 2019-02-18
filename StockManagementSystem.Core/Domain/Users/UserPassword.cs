using System;

namespace StockManagementSystem.Core.Domain.Users
{
    public class UserPassword : BaseEntity
    {
        public UserPassword()
        {
            this.PasswordFormat = PasswordFormat.Clear;
        }

        public int UserId { get; set; }

        public string Password { get; set; }

        public int PasswordFormatId { get; set; }

        public string PasswordSalt { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public PasswordFormat PasswordFormat
        {
            get => (PasswordFormat)PasswordFormatId;
            set => PasswordFormatId = (int)value;
        }

        public virtual User User { get; set; }
    }
}