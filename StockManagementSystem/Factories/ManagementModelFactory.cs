using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Management;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Management;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Extensions;
using StockManagementSystem.Web.Kendoui.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Factories
{
    public class ManagementModelFactory : IManagementModelFactory
    {
        private readonly IOutletManagementService _outletManagementService;
        private readonly IStoreService _storeService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public ManagementModelFactory(
            IOutletManagementService outletManagementService,
            IStoreService storeService,
            IUserService userService,
             IDateTimeHelper dateTimeHelper)
        {
            _outletManagementService = outletManagementService;
            _storeService = storeService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
        }

        #region Outlet Management

        public async Task<OutletManagementContainerModel> PrepareOutletManagementContainerModel(
            OutletManagementContainerModel outletManagementContainerModel)
        {
            if (outletManagementContainerModel == null)
                throw new ArgumentNullException(nameof(outletManagementContainerModel));

            //prepare nested models
            await PrepareAssignUserListModel(outletManagementContainerModel.AssignUserList);
            await PrepareGroupOutletListModel(outletManagementContainerModel.GroupOutletList);

            return outletManagementContainerModel;
        }

        #endregion

        #region Assign User

        public async Task<AssignUserSearchModel> PrepareAssignUserSearchModel(AssignUserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            //store
            var stores = await _storeService.GetStoresAsync();
            searchModel.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo.ToString() + " - " + store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            //user
            var users = await _userService.GetUsersAsync();
            searchModel.AvailableUsers = users.Select(user => new SelectListItem
            {
                Text = user.Username,
                Value = user.Id.ToString()
            }).ToList();

            return await Task.FromResult(searchModel);
        }

        public async Task<AssignUserListModel> PrepareAssignUserListModel(AssignUserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var assignUsers = await _outletManagementService.GetAssignUsersAsync(
                storeIds: searchModel.SelectedStoreId.ToArray(),
                userIds: searchModel.SelectedUserIds.ToArray(),
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            if (assignUsers == null)
                throw new ArgumentNullException(nameof(assignUsers));

            var model = new AssignUserListModel
            {
                Data = assignUsers.Select(assignUser =>
                //Data = assignUsers.PaginationByRequestModel(searchModel).Select(assignUser =>
                {
                    var assignUsersModel = assignUser.ToModel<AssignUserModel>();

                    assignUsersModel.SelectedStoreId = assignUser.StoreId;
                    assignUsersModel.StoreName = assignUser.Store.P_BranchNo + " - " + assignUser.Store.P_Name;
                    assignUsersModel.SelectedUserIds = assignUser.StoreUserAssignStore.Select(map => map.UserId).ToList();
                    ////assignUsersModel.SelectedUserIds = assignUser.StoreUserAssignStore.Contains(user => user.UserId);
                    assignUsersModel.User = String.Join(", ", assignUser.StoreUserAssignStore.Select(user => user.User.Username));
                    assignUsersModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(assignUser.CreatedOnUtc, DateTimeKind.Utc);
                    assignUsersModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(assignUser.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);

                    return assignUsersModel;
                }),
                Total = assignUsers.Count
            };

            // sort
            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            // filter
            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }
        
        public async Task<AssignUserModel> PrepareAssignUserListbyStoreModel(int storeID)
        {
            var assignUsers = await _outletManagementService.GetAssignUsersByStoreIdAsync(storeID);

            var model = new AssignUserModel
            {
                StoreUsers = assignUsers
            };

            return model;
        }

        public async Task<AssignUserModel> PrepareAssignUserListModel()
        {
            var assignUsers = await _outletManagementService.GetAllAssignUsersAsync();

            var model = new AssignUserModel
            {
                StoreUsers = assignUsers
            };

            return model;
        }

        public async Task<AssignUserModel> PrepareAssignUserModel(AssignUserModel model, StoreUserAssign storeUserAssign)
        {
            if (storeUserAssign != null)
            {
                model = model ?? new AssignUserModel();

                model.Id = storeUserAssign.Id;
                model.StoreName = storeUserAssign.Store.P_BranchNo + " - " + storeUserAssign.Store.P_Name;
                model.SelectedStoreId = storeUserAssign.StoreId;
                model.User = storeUserAssign.StoreUserAssignStore.Select(map => map.User.Username).ToString();
                model.User = storeUserAssign.StoreUserAssignStore.Select(map => map.User.Username).ToString();
                model.SelectedUserIds = storeUserAssign.StoreUserAssignStore.Select(map => map.UserId).ToList();
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(storeUserAssign.CreatedOnUtc, DateTimeKind.Utc);
                model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(storeUserAssign.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);
                model.SelectedStoreId = storeUserAssign.Store.Id;
            }

            //store
            var stores = await _storeService.GetStoresAsync();
            model.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo.ToString() + " - " + store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            //users
            var users = await _userService.GetUsersAsync();
            model.AvailableUsers = users.Select(user => new SelectListItem
            {
                Text = user.Username,
                Value = user.Id.ToString()
            }).ToList();

            return await Task.FromResult(model);
        }

        #endregion

        #region Group Outlet

        public async Task<GroupOutletSearchModel> PrepareGroupOutletSearchModel(GroupOutletSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            var stores = await _storeService.GetStoresAsync();
            searchModel.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo.ToString() + " - " + store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            return await Task.FromResult(searchModel);
        }

        public async Task<GroupOutletListModel> PrepareGroupOutletListModel(GroupOutletSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var groupList = await _outletManagementService.GetGroupOutletsAsync(
                storeIds: searchModel.SelectedStoreIds.ToArray(),
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            if (groupList == null)
                throw new ArgumentNullException(nameof(groupList));

            var model = new GroupOutletListModel
            {
                //Data = groupList.PaginationByRequestModel(searchModel).Select(groupOutlet =>
                Data = groupList.Select(groupOutlet =>
                {
                    var groupOutletsModel = groupOutlet.ToModel<GroupOutletModel>();

                    groupOutletsModel.GroupName = groupOutlet.GroupName;
                    groupOutletsModel.StoreName = String.Join(", ", groupOutlet.StoreGroupingStore.Select(store => store.Store.P_BranchNo + " - " + store.Store.P_Name));
                    groupOutletsModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(groupOutlet.CreatedOnUtc, DateTimeKind.Utc);
                    groupOutletsModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(groupOutlet.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);

                    return groupOutletsModel;
                }),
              Total = groupList.Count
            };

            // sort
            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            // filter
            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        public async Task<GroupOutletModel> PrepareGroupOutletListModel()
        {
            var groupOutlets = await _outletManagementService.GetAllGroupOutletsAsync();

            var model = new GroupOutletModel
            {
                StoreGroupings = groupOutlets
            };

            return model;
        }

        public async Task<GroupOutletModel> PrepareGroupOutletModel(GroupOutletModel model, StoreGrouping storeGrouping)
        {
            if (storeGrouping != null)
            {
                model = model ?? new GroupOutletModel();

                model.Id = storeGrouping.Id;
                model.GroupName = storeGrouping.GroupName;
                model.SelectedStoreIds = storeGrouping.StoreGroupingStore.Select(sgs => sgs.StoreId).ToList();
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(storeGrouping.CreatedOnUtc, DateTimeKind.Utc);
                model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(storeGrouping.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);
            }

            var stores = await _storeService.GetStoresAsync();
            model.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo.ToString() + " - " + store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            return await Task.FromResult(model);
        }

        #endregion
    }
}
