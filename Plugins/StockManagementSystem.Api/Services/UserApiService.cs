using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Services
{
    public class UserApiService : IUserApiService
    {
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string DateOfBirth = "dateofbirth";
        private const string Gender = "gender";

        private readonly ITenantContext _tenantContext;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;

        public UserApiService(
            IRepository<User> userRepository, 
            IRepository<GenericAttribute> genericAttributeRepository,
            ITenantContext tenantContext)
        {
            _userRepository = userRepository;
            _genericAttributeRepository = genericAttributeRepository;
            _tenantContext = tenantContext;
        }

        public IList<UserDto> GetUserDtos(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetUsersQuery(createdAtMin, createdAtMax, sinceId);

            var result = HandleUserGenericAttributes(null, query, limit, page);

            return result;
        }

        public int GetUsersCount()
        {
            return _userRepository.Table.Count(user =>
                !user.Deleted && (user.RegisteredInTenantId == 0 ||
                                  user.RegisteredInTenantId == _tenantContext.CurrentTenant.Id));
        }

        public IList<UserDto> Search(string queryParams = "", string order = Configurations.DefaultOrder,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit)
        {
            IList<UserDto> result = new List<UserDto>();

            var searchParams = EnsureSearchQueryIsValid(queryParams, ParseSearchQuery);
            if (searchParams != null)
            {
                var query = _userRepository.Table.Where(user => !user.Deleted);

                foreach (var searchParam in searchParams)
                {
                    // Skip non existing properties.
                    if (ReflectionHelper.HasProperty(searchParam.Key, typeof(User)))
                        query = query.Where(string.Format("{0} = @0 || {0}.Contains(@0)", searchParam.Key),
                            searchParam.Value);
                }

                result = HandleUserGenericAttributes(searchParams, query, limit, page, order);
            }

            return result;
        }

        public Dictionary<string, string> GetFirstAndLastNameByUserId(int userId)
        {
            return _genericAttributeRepository.Table.Where(
                    x => x.KeyGroup == "User" && x.EntityId == userId && (x.Key == FirstName || x.Key == LastName))
                .ToDictionary(x => x.Key.ToLowerInvariant(), y => y.Value);
        }

        public User GetUserEntityById(int id)
        {
            var user = _userRepository.Table.FirstOrDefault(u => u.Id == id && !u.Deleted);

            return user;
        }

        public UserDto GetUserById(int id, bool showDeleted = false)
        {
            if (id == 0)
                return null;

            var userAttributeMappings =
            (from user in _userRepository.Table
                join attribute in _genericAttributeRepository.Table
                    on user.Id equals attribute.EntityId
                where user.Id == id && attribute.KeyGroup == "User"
                select new UserAttributeMappingDto()
                {
                    Attribute = attribute,
                    User = user
                }).ToList();

            UserDto userDto = null;

            if (userAttributeMappings.Count > 0)
            {
                var user = userAttributeMappings.First().User;
                userDto = user.ToDto();

                foreach (var mapping in userAttributeMappings)
                {
                    if (!showDeleted && mapping.User.Deleted)
                        continue;

                    if (mapping.Attribute != null)
                    {
                        if (mapping.Attribute.Key.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase))
                            userDto.FirstName = mapping.Attribute.Value;
                        else if (mapping.Attribute.Key.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))
                            userDto.LastName = mapping.Attribute.Value;
                        else if(mapping.Attribute.Key.Equals(DateOfBirth, StringComparison.InvariantCultureIgnoreCase))
                            userDto.DateOfBirth = string.IsNullOrEmpty(mapping.Attribute.Value) ? (DateTime?)null : DateTime.Parse(mapping.Attribute.Value);
                        else if(mapping.Attribute.Key.Equals(Gender, StringComparison.InvariantCultureIgnoreCase))
                            userDto.Gender = mapping.Attribute.Value;

                    }
                }
            }
            else
            {
                // This is when we do not have first and last name set.
                var currentUser = _userRepository.Table.FirstOrDefault(user => user.Id == id);

                if (currentUser != null)
                {
                    if (showDeleted || !currentUser.Deleted)
                        userDto = currentUser.ToDto();
                }
            }

            return userDto;
        }

        private Dictionary<string, string> EnsureSearchQueryIsValid(string query, Func<string, Dictionary<string, string>> parseSearchQuery)
        {
            if (!string.IsNullOrEmpty(query))
            {
                return parseSearchQuery(query);
            }

            return null;
        }

        private Dictionary<string, string> ParseSearchQuery(string query)
        {
            var parsedQuery = new Dictionary<string, string>();

            var splitPattern = @"(\w+):";

            var fieldValueList = Regex.Split(query, splitPattern).Where(s => s != String.Empty).ToList();

            if (fieldValueList.Count < 2)
            {
                return parsedQuery;
            }

            for (var i = 0; i < fieldValueList.Count; i += 2)
            {
                var field = fieldValueList[i];
                var value = fieldValueList[i + 1];

                if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(value))
                {
                    field = field.Replace("_", string.Empty);
                    parsedQuery.Add(field.Trim(), value.Trim());
                }
            }

            return parsedQuery;
        }

        private IList<UserDto> HandleUserGenericAttributes(IReadOnlyDictionary<string, string> searchParams,
            IQueryable<User> query,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            string order = Configurations.DefaultOrder)
        {
            var allRecordsGroupedByUserId =
            (from user in query
                from attribute in _genericAttributeRepository.Table
                    .Where(attr => attr.EntityId == user.Id &&
                                   attr.KeyGroup == "User").DefaultIfEmpty()
                select new UserAttributeMappingDto
                {
                    Attribute = attribute,
                    User = user
                }).GroupBy(x => x.User.Id);

            if (searchParams != null && searchParams.Count > 0)
            {
                if (searchParams.ContainsKey(FirstName))
                    allRecordsGroupedByUserId = GetUserAttributesMappingsByKey(allRecordsGroupedByUserId, FirstName,
                        searchParams[FirstName]);

                if (searchParams.ContainsKey(LastName))
                    allRecordsGroupedByUserId =
                        GetUserAttributesMappingsByKey(allRecordsGroupedByUserId, LastName, searchParams[LastName]);

                if (searchParams.ContainsKey(DateOfBirth))
                    allRecordsGroupedByUserId = GetUserAttributesMappingsByKey(allRecordsGroupedByUserId, DateOfBirth,
                        searchParams[DateOfBirth]);

                if (searchParams.ContainsKey(Gender))
                    allRecordsGroupedByUserId =
                        GetUserAttributesMappingsByKey(allRecordsGroupedByUserId, Gender, searchParams[Gender]);
            }

            var result = GetFullUserDtos(allRecordsGroupedByUserId, page, limit, order);

            return result;
        }

        private IList<UserDto> GetFullUserDtos(
            IQueryable<IGrouping<int, UserAttributeMappingDto>> userAttributesMappings,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit,
            string order = Configurations.DefaultOrder)
        {
            var userDtos = new List<UserDto>();

            userAttributesMappings = userAttributesMappings.OrderBy(x => x.Key);

            IList<IGrouping<int, UserAttributeMappingDto>> userAttributeGroupsList =
                new ApiList<IGrouping<int, UserAttributeMappingDto>>(userAttributesMappings, page - 1, limit);

            foreach (var group in userAttributeGroupsList)
            { 
                IList<UserAttributeMappingDto> mappingsForMerge = group.Select(x => x).ToList();
                var userDto = Merge(mappingsForMerge);

                userDtos.Add(userDto);
            }

            return userDtos.AsQueryable().OrderBy(order).ToList();
        }

        private static UserDto Merge(IList<UserAttributeMappingDto> mappingsForMerge)
        {
            var userDto = mappingsForMerge.First().User.ToDto();

            var attributes = mappingsForMerge.Select(x => x.Attribute).ToList();

            foreach (var attribute in attributes)
            {
                if (attribute != null)
                {
                    if (attribute.Key.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase))
                        userDto.FirstName = attribute.Value;
                    else if (attribute.Key.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))
                        userDto.LastName = attribute.Value;
                    else if (attribute.Key.Equals(DateOfBirth, StringComparison.InvariantCultureIgnoreCase))
                        userDto.DateOfBirth = string.IsNullOrEmpty(attribute.Value) ? (DateTime?)null : DateTime.Parse(attribute.Value);
                    else if (attribute.Key.Equals(Gender, StringComparison.InvariantCultureIgnoreCase))
                        userDto.Gender = attribute.Value;
                }
            }

            return userDto;
        }

        private IQueryable<IGrouping<int, UserAttributeMappingDto>> GetUserAttributesMappingsByKey(
            IQueryable<IGrouping<int, UserAttributeMappingDto>> userAttributesGroups, string key, string value)
        {
            // filter the userAttributesGroups to be only the ones that have the passed key parameter as a key.
            var userAttributesMappingByKey = from @group in userAttributesGroups
                                             where @group.Select(x => x.Attribute)
                                                .Any(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) && 
                                                x.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                                             select @group;

            return userAttributesMappingByKey;
        }

        private IQueryable<User> GetUsersQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int sinceId = 0)
        {
            var query = _userRepository.Table.Where(user => !user.Deleted && !user.IsSystemAccount && user.Active);

            query = query.Where(user =>
                !user.UserRoles.Any(ur => ur.Role.Active && ur.Role.SystemName == UserDefaults.GuestsRoleName) &&
                (user.RegisteredInTenantId == 0 || user.RegisteredInTenantId == _tenantContext.CurrentTenant.Id));

            if (createdAtMin != null)
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null)
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);

            query = query.OrderBy(user => user.Id);

            if (sinceId > 0)
                query = query.Where(user => user.Id > sinceId);

            return query;
        }
    }
}