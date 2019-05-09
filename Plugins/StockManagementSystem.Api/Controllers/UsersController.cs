using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Delta;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.ModelBinders;
using StockManagementSystem.Api.Models.UsersParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : BaseApiController
    {
        private readonly IUserApiService _userApiService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IEncryptionService _encryptionService;
        private readonly IStoreService _storeService;
        private readonly IFactory<User> _factory;

        // Mocking testing requirements - not support concrete types as dependencies 
        private UserSettings _userSettings;
        private UserSettings UserSettings => _userSettings ?? (_userSettings = EngineContext.Current.Resolve<UserSettings>());

        public UsersController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            IUserService userService,
            ITenantMappingService tenantMappingService,
            ITenantService tenantService,
            IUserActivityService userActivityService,
            IUserApiService userApiService,
            IGenericAttributeService genericAttributeService,
            IEncryptionService encryptionService,
            IStoreService storeService,
            IFactory<User> factory)
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _userApiService = userApiService;
            _genericAttributeService = genericAttributeService;
            _encryptionService = encryptionService;
            _storeService = storeService;
            _factory = factory;
        }

        /// <summary>
        /// Retrieve all users
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/users")]
        [ProducesResponseType(typeof(UsersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetUsers(UsersParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var users = _userApiService.GetUserDtos(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Limit,
                parameters.Page, parameters.SinceId, parameters.RoleIds, parameters.StoreIds);

            var usersRootObject = new UsersRootObject
            {
                Users = users
            };

            var json = JsonFieldsSerializer.Serialize(usersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve user by id
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="fields">Fields from the user you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/users/{id}")]
        [ProducesResponseType(typeof(UsersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetUserById(int id, string fields = "")
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var user = _userApiService.GetUserById(id);
            if (user == null)
                return await Error(HttpStatusCode.NotFound, "user", "not found");

            var usersRootObject = new UsersRootObject();
            usersRootObject.Users.Add(user);

            var json = JsonFieldsSerializer.Serialize(usersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Get a count of all users
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/users/count")]
        [ProducesResponseType(typeof(UsersCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUsersCount()
        {
            var usersCount = _userApiService.GetUsersCount();

            var usersCountRootObject = new UsersCountRootObject()
            {
                Count = usersCount
            };

            return await Task.FromResult<IActionResult>(Ok(usersCountRootObject));
        }

        /// <summary>
        /// Search for users matching supplied query
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/users/search")]
        [ProducesResponseType(typeof(UsersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Search(UsersSearchParametersModel parameters)
        {
            if (parameters.Limit <= Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page <= 0)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            var usersDto = _userApiService.Search(parameters.Query, parameters.Order, parameters.Page, parameters.Limit);

            var usersRootObject = new UsersRootObject()
            {
                Users = usersDto
            };

            var json = JsonFieldsSerializer.Serialize(usersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Create new user
        /// </summary>
        [HttpPost]
        [Route("/api/users")]
        [ProducesResponseType(typeof(UsersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateUser([ModelBinder(typeof(JsonModelBinder<UserDto>))] Delta<UserDto> userDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var newUser = _factory.Initialize();
            userDelta.Merge(newUser);

            await UserService.InsertUserAsync(newUser);
            await InsertFirstAndLastNameGenericAttributes(userDelta.Dto.FirstName, userDelta.Dto.LastName, newUser);

            //password 
            if (!string.IsNullOrWhiteSpace(userDelta.Dto.Password))
                await AddPassword(userDelta.Dto.Password, newUser);

            //roles
            if (userDelta.Dto.RoleIds.Count > 0)
                AddValidRoles(userDelta, newUser);

            //stores
            if (userDelta.Dto.StoreIds.Count > 0)
                await AddValidStores(userDelta, newUser);

            await UserService.UpdateUserAsync(newUser);

            var newUserDto = newUser.ToDto();
            newUserDto.FirstName = userDelta.Dto.FirstName;
            newUserDto.LastName = userDelta.Dto.LastName;
            newUserDto.UserPassword = _userApiService.GetUserPassword(newUserDto.Id);

            //activity log
            await UserActivityService.InsertActivityAsync("AddNewUser", $"Added a new user (ID = {newUser.Id})", newUser);

            var usersRootObject = new UsersRootObject();
            usersRootObject.Users.Add(newUserDto);

            var json = JsonFieldsSerializer.Serialize(usersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Update user by id
        /// </summary>
        [HttpPut]
        [Route("/api/users/{id}")]
        [ProducesResponseType(typeof(UsersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateUser([ModelBinder(typeof(JsonModelBinder<UserDto>))] Delta<UserDto> userDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var currentUser = _userApiService.GetUserEntityById(userDelta.Dto.Id);
            if (currentUser == null)
                return await Error(HttpStatusCode.NotFound, "user", "not found");

            userDelta.Merge(currentUser);

            //password
            if (!string.IsNullOrWhiteSpace(userDelta.Dto.Password))
                await AddPassword(userDelta.Dto.Password, currentUser);

            //roles
            if (userDelta.Dto.RoleIds.Count > 0)
                AddValidRoles(userDelta, currentUser);

            //stores
            if (userDelta.Dto.StoreIds.Count > 0)
                await AddValidStores(userDelta, currentUser);

            await UserService.UpdateUserAsync(currentUser);
            await InsertFirstAndLastNameGenericAttributes(userDelta.Dto.FirstName, userDelta.Dto.LastName, currentUser);

            // Preparing the result dto of the new user
            var updatedUser = currentUser.ToDto();

            var firstNameGenericAttribute =
                (await _genericAttributeService.GetAttributesForEntityAsync(currentUser.Id, typeof(User).Name))
                .FirstOrDefault(x => x.Key == "FirstName");
            if (firstNameGenericAttribute != null)
                updatedUser.FirstName = firstNameGenericAttribute.Value;

            var lastNameGenericAttribute =
                (await _genericAttributeService.GetAttributesForEntityAsync(currentUser.Id, typeof(User).Name))
                .FirstOrDefault(x => x.Key == "LastName");
            if (lastNameGenericAttribute != null)
                updatedUser.LastName = lastNameGenericAttribute.Value;

            updatedUser.UserPassword = _userApiService.GetUserPassword(updatedUser.Id);

            //activity log
            await UserActivityService.InsertActivityAsync("EditUser", $"Edited a user (ID = {currentUser.Id})", currentUser);

            var usersRootObject = new UsersRootObject();
            usersRootObject.Users.Add(updatedUser);

            var json = JsonFieldsSerializer.Serialize(usersRootObject, String.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Delete user by id
        /// </summary>
        [HttpDelete]
        [Route("/api/users/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "Invalid id");

            var user = _userApiService.GetUserEntityById(id);
            if (user == null)
                return await Error(HttpStatusCode.NotFound, "user", "User not found");

            await UserService.DeleteUserAsync(user);

            //activity log
            await UserActivityService.InsertActivityAsync("DeleteUser", $"Deleted a user (ID = {id})", user);

            return new RawJsonActionResult("{}");
        }

        #region Private methods

        private async Task InsertFirstAndLastNameGenericAttributes(string firstName, string lastName, User newUser)
        {
            if (firstName != null)
                await _genericAttributeService.SaveAttributeAsync(newUser, UserDefaults.FirstNameAttribute, firstName);

            if (lastName != null)
                await _genericAttributeService.SaveAttributeAsync(newUser, UserDefaults.LastNameAttribute, lastName);
        }

        private void AddValidRoles(Delta<UserDto> userDelta, User currentUser)
        {
            var roles = UserService.GetRoles(true);
            foreach (var role in roles)
            {
                if (userDelta.Dto.RoleIds.Contains(role.Id))
                {
                    if (currentUser.UserRoles.Count(mapping => mapping.RoleId == role.Id) == 0)
                        currentUser.AddUserRole(new UserRole { Role = role });
                }
                else
                {
                    if (currentUser.UserRoles.Count(mapping => mapping.RoleId == role.Id) > 0)
                        currentUser.RemoveUserRole(
                            currentUser.UserRoles.FirstOrDefault(mapping => mapping.RoleId == role.Id));
                }
            }
        }

        private async Task AddValidStores(Delta<UserDto> userDelta, User currentUser)
        {
            var stores = await _storeService.GetStores();
            foreach (var store in stores)
            {
                if (userDelta.Dto.StoreIds.Contains(store.P_BranchNo))
                {
                    if (currentUser.UserStores.Count(mapping => mapping.StoreId == store.P_BranchNo) == 0)
                        currentUser.UserStores.Add(new UserStore { Store = store });
                }
                else
                {
                    if (currentUser.UserStores.Count(mapping => mapping.StoreId == store.P_BranchNo) > 0)
                        currentUser.UserStores.Remove(currentUser.UserStores.FirstOrDefault(mapping => mapping.StoreId == store.P_BranchNo));
                }
            }
        }

        private async Task AddPassword(string newPassword, User user)
        {
            var userPassword = new UserPassword()
            {
                User = user,
                PasswordFormat = UserSettings.DefaultPasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };

            switch (UserSettings.DefaultPasswordFormat)
            {
                case PasswordFormat.Clear:
                    userPassword.Password = newPassword;
                    break;
                case PasswordFormat.Encrypted:
                    userPassword.Password = _encryptionService.EncryptText(newPassword);
                    break;
                case PasswordFormat.Hashed:
                    {
                        var saltKey = _encryptionService.CreateSaltKey(5);
                        userPassword.PasswordSalt = saltKey;
                        userPassword.Password = _encryptionService.CreatePasswordHash(newPassword, saltKey, UserSettings.HashedPasswordFormat);
                    }
                    break;
            }

            UserService.InsertUserPassword(userPassword);

            await UserService.UpdateUserAsync(user);
        }

        #endregion
    }
}