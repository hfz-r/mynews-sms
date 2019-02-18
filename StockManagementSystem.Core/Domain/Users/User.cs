using System;
using System.Collections.Generic;
using System.Linq;

namespace StockManagementSystem.Core.Domain.Users
{
    public class User : BaseEntity
    {
        private IList<Role> _roles;
        private ICollection<UserRole> _userRoles;

        public User()
        {
            UserGuid = Guid.NewGuid();
        }

        public Guid UserGuid { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating number of failed login attempts (wrong password)
        /// </summary>
        public int FailedLoginAttempts { get; set; }

        /// <summary>
        /// Gets or sets the date and time until which a user cannot login (locked out)
        /// </summary>
        public DateTime? CannotLoginUntilDateUtc { get; set; }

        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        public bool IsSystemAccount { get; set; }

        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }

        /// <summary>
        ///  Gets or sets the store identifier in which user registered
        /// </summary>
        public int RegisteredInStoreId { get; set; }

        #region Navigation properties

        public virtual IList<Role> Roles => _roles ?? (_roles = UserRoles.Select(mapping => mapping.Role).ToList());

        public virtual ICollection<UserRole> UserRoles
        {
            get => _userRoles ?? (_userRoles = new List<UserRole>());
            protected set => _userRoles = value;
        }

        #endregion

        #region Methods

        public void AddUserRole(UserRole userRole)
        {
            UserRoles.Add(userRole);
            _roles = null;
        }

        public void RemoveUserRole(UserRole userRole)
        {
            UserRoles.Remove(userRole);
            _roles = null;
        }

        #endregion
    }
}