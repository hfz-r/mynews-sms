using System;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Stores;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Kendoui.Extensions;

namespace StockManagementSystem.Factories
{
    public class StoreModelFactory : IStoreModelFactory
    {
        private readonly IBaseModelFactory _baseModelFactory;
        private readonly IStoreService _storeService;
        private readonly IUserService _userService;

        public StoreModelFactory(
            IBaseModelFactory baseModelFactory,
            IStoreService storeService,
            IUserService userService)
        {
            _baseModelFactory = baseModelFactory;
            _storeService = storeService;
            _userService = userService;
        }

        protected virtual Task<UserStoreSearchModel> PrepareUserStoreSearchModel(UserStoreSearchModel searchModel, Store store)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (store == null)
                throw new ArgumentNullException(nameof(store));

            searchModel.StoreId = store.P_BranchNo;

            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        public async Task<StoreSearchModel> PrepareStoreSearchModel(StoreSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            await _baseModelFactory.PrepareStoreAreaCodes(searchModel.AvailableAreaCodes);
            await _baseModelFactory.PrepareStoreCities(searchModel.AvailableCities);
            await _baseModelFactory.PrepareStoreStates(searchModel.AvailableStates);

            searchModel.SetGridPageSize();

            return searchModel;
        }

        public async Task<StoreListModel> PrepareStoreListModel(StoreSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get stores
            var stores = await _storeService.GetAllStores(
                storeName: searchModel.SearchStoreName,
                areaCode: searchModel.SearchAreaCode,
                city: searchModel.SearchCity,
                state: searchModel.SearchState,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new StoreListModel
            {
                Data = stores.Select(store =>
                {
                    var storeModel = store.ToModel<StoreModel>();

                    storeModel.BranchNo = store.P_BranchNo;
                    storeModel.Name = store.P_Name;
                    storeModel.AreaCode = store.P_AreaCode;
                    storeModel.Address1 = store.P_Addr1;
                    storeModel.Address2 = store.P_Addr2;
                    storeModel.Address3 = store.P_Addr3;
                    storeModel.City = store.P_City;
                    storeModel.State = store.P_State;
                    storeModel.Country = store.P_Country;
                    storeModel.CountUserStore = store.UserStores.Count;

                    return storeModel;
                }),
                Total = stores.TotalCount
            };

            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        /// <summary>
        /// Prepare store model
        /// </summary>
        /// <param name="model">Store model</param>
        /// <param name="store">Store</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns></returns>
        public async Task<StoreModel> PrepareStoreModel(StoreModel model, Store store, bool excludeProperties = false)
        {
            if (store != null)
            {
                model = model ?? store.ToModel<StoreModel>();
                model.BranchNo = store.P_BranchNo;

                if (!excludeProperties)
                {
                    model.Name = store.P_Name;
                    model.AreaCode = store.P_AreaCode;
                    model.Address1 = store.P_Addr1;
                    model.Address2 = store.P_Addr2;
                    model.Address3 = store.P_Addr3;
                    model.City = store.P_City;
                    model.State = store.P_State;
                    model.Country = store.P_Country;
                }

                //prepare nested search models
                await PrepareUserStoreSearchModel(model.UserStoreSearchModel, store);
            }

            if (store == null)
                model.Active = false;

            await _baseModelFactory.PrepareStoreAreaCodes(model.AvailableAreaCodes);
            await _baseModelFactory.PrepareStoreCities(model.AvailableCities);
            await _baseModelFactory.PrepareStoreStates(model.AvailableStates);

            return model;
        }

        public async Task<UserStoreListModel> PrepareUserStoreListModel(UserStoreSearchModel searchModel, Store store)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var usersStore = await _storeService.GetUsersStore(storeId: store.P_BranchNo, pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new UserStoreListModel
            {
                Data = usersStore.Select(user =>
                {
                    var userStoreModel = user.ToModel<UserStoreModel>();
                    userStoreModel.UserId = user.Id;
                    userStoreModel.Email = user.Email;
                    userStoreModel.Username = user.Username;
                    userStoreModel.Active = user.Active;

                    return userStoreModel;
                }),
                Total = usersStore.TotalCount
            };

            return model;
        }

        public virtual Task<AddUserToStoreSearchModel> PrepareAddUserToStoreSearchModel(AddUserToStoreSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetPopupGridPageSize();

            return Task.FromResult(searchModel);
        }

        public async Task<AddUserToStoreListModel> PrepareAddUserToStoreListModel(AddUserToStoreSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var users = await _userService.GetUsersAsync(username: searchModel.SearchUsername, email: searchModel.SearchEmail,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new AddUserToStoreListModel
            {
                Data = users.Select(user =>
                {
                    var userModel = user.ToModel<UserModel>();
                    userModel.Email = user.Email;
                    userModel.Username = user.Username;
                    userModel.Active = user.Active;

                    return userModel;
                }),
                Total = users.TotalCount
            };

            return model;
        }

    }
}