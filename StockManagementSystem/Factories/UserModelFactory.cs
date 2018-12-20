using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Factories
{
    /// <summary>
    /// Represents the user model factory implementation
    /// </summary>
    public class UserModelFactory
    {
        private readonly IUserService _userService;

        public UserModelFactory(
            IUserService userService)
        {
            _userService = userService;
        }

        //public async Task<UserSearchModel> PrepareUserSearcModelAsync(UserSearchModel searchModel)
        //{
        //    if (searchModel == null)
        //        throw new ArgumentNullException(nameof(searchModel));


        //}

        //public async Task<IEnumerable<UserSearchModel>> PrepareUserSearcModelAsync()
        //{
        //    var users = await _userService.GetAllUsersAsync();

        //    if (users == null)
        //        return Enumerable.Empty<UserSearchModel>();

        //    var sm = new List<UserSearchModel>();

        //    foreach (var user in users)
        //    {
        //        sm.Add(new UserSearchModel
        //        {

        //        });
        //    }
        //}
    }
}