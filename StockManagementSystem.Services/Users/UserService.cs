using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _userRepository;

        public UserService(
            UserManager<User> userManager, 
            IRepository<User> userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        //public async Task<IEnumerable<User>> GetAllUsersAsync()
        //{
        //    var users = await _userManager.Users.ToListAsync();

        //    if (users == null)


        //    return users;
        //}
    }
}