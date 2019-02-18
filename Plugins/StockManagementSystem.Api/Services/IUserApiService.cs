using System;
using System.Collections.Generic;
using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Services
{
    public interface IUserApiService
    {
        Dictionary<string, string> GetFirstAndLastNameByUserId(int userId);

        UserDto GetUserById(int id, bool showDeleted = false);

        IList<UserDto> GetUserDtos(DateTime? createdAtMin = null, DateTime? createdAtMax = null, int limit = 50,
            int page = 1, int sinceId = 0);

        User GetUserEntityById(int id);

        int GetUsersCount();

        IList<UserDto> Search(string queryParams = "", string order = "Id", int page = 1, int limit = 50);
    }
}