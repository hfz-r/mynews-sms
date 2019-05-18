using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Core.Domain.Users
{
    public class User : BaseEntity, IAppendTimestamps, IAppliedStoreSupported
    {
        private IList<Role> _roles;
        private ICollection<UserRole> _userRoles;
        private ICollection<UserStore> _userStores;

        public User()
        {
            UserGuid = Guid.NewGuid();
        }

        public Guid UserGuid { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Barcode { get; set; }

        public bool LoginToken { get; set; } //for HHT 

        public DateTime? PasswordLastModified { get; set; } //for HHT 

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
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }

        /// <summary>
        ///  Gets or sets the tenant identifier in which user registered
        /// </summary>
        public int RegisteredInTenantId { get; set; }

        public virtual ICollection<StoreUserAssignStores> StoreUserAssignStore { get; set; }

        #region Navigation properties

        public virtual IList<Role> Roles => _roles ?? (_roles = UserRoles.Select(mapping => mapping.Role).ToList());

        public virtual ICollection<UserRole> UserRoles
        {
            get => _userRoles ?? (_userRoles = new List<UserRole>());
            protected set => _userRoles = value;
        }

        public virtual IList<Store> AppliedStores => UserStores.Select(mapping => mapping.Store).ToList();

        public virtual ICollection<UserStore> UserStores
        {
            get => _userStores ?? (_userStores = new List<UserStore>());
            set => _userStores = value;
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

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}