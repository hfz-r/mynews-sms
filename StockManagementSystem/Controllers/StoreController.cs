using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Factories;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Stores;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;

namespace StockManagementSystem.Controllers
{
    public class StoreController : BaseController
    {
        private readonly IStoreModelFactory _storeModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IUserActivityService _userActivityService;

        public StoreController(
            IStoreModelFactory storeModelFactory,
            IPermissionService permissionService,
            IStoreService storeService,
            IUserService userService,
            INotificationService notificationService, 
            IUserActivityService userActivityService)
        {
            _storeModelFactory = storeModelFactory;
            _permissionService = permissionService;
            _storeService = storeService;
            _userService = userService;
            _notificationService = notificationService;
            _userActivityService = userActivityService;
        }

        #region Utilities

        protected async Task<bool> DeleteStore(int id)
        {
            var store = _storeService.GetStoreById(id);
            if (store == null)
                return false;

            await _storeService.DeleteStore(store);

            return true;
        }

        #endregion

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedView();

            var model = await _storeModelFactory.PrepareStoreSearchModel(new StoreSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> List(StoreSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedKendoGridJson();

            var model = await _storeModelFactory.PrepareStoreListModel(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(id);
            if (store == null)
                return RedirectToAction("List");

            var model = await _storeModelFactory.PrepareStoreModel(null, store);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(StoreModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(model.BranchNo);
            if (store == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                store = model.ToEntity(store);

                store.P_BranchNo = model.BranchNo;
                store.P_Name = model.Name;
                store.P_AreaCode = model.AreaCode;
                store.P_Addr1 = model.Address1;
                store.P_Addr2 = model.Address2;
                store.P_Addr3 = model.Address3;
                store.P_City = model.City;
                store.P_State = model.State;
                store.P_Country = model.Country;

                await _storeService.UpdateStore(store);

                await _userActivityService.InsertActivityAsync("EditStore", $"Edited a store (ID = {store.P_BranchNo})", store);

                _notificationService.SuccessNotification("Store has been updated successfully.");

                if (!continueEditing)
                    return RedirectToAction("List");

                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = store.Id });
            }

            model = await _storeModelFactory.PrepareStoreModel(model, store, true);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteInline(int branchNo)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedView();

            var result = await DeleteStore(branchNo);
            if (!result)
                throw new ArgumentException("No store found with the specified id", nameof(branchNo));

            await _userActivityService.InsertActivityAsync("DeleteStore", $"Deleted a store (ID = {branchNo})");

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedView();

            var result = await DeleteStore(id);
            if (!result)
                return RedirectToAction("List");

            await _userActivityService.InsertActivityAsync("DeleteStore", $"Deleted a store (ID = {id})");

            _notificationService.SuccessNotification("Store has been deleted successfully.");

            return RedirectToAction("List");
        }

        #region UserStore

        [HttpPost]
        public async Task<IActionResult> UserStoreList(UserStoreSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedKendoGridJson();

            var store = _storeService.GetStoreById(searchModel.StoreId) ??
                        throw new ArgumentException("No store found with the specified id");

            var model = await _storeModelFactory.PrepareUserStoreListModel(searchModel, store);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserStoreDelete(int storeId, int userId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(storeId) ??
                        throw new ArgumentException("No store found with the specified id", nameof(storeId));

            var user = await _userService.GetUserByIdAsync(userId) ??
                       throw new ArgumentException("No user found with the specified id", nameof(userId));

            if (user.UserStores.Count(mapping => mapping.StoreId == store.P_BranchNo) > 0)
                user.UserStores.Remove(user.UserStores.FirstOrDefault(mapping => mapping.StoreId == store.P_BranchNo));

            await _userService.UpdateUserAsync(user);

            return new NullJsonResult();
        }

        public async Task<IActionResult> UserAddPopup(int storeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedView();

            var model = await _storeModelFactory.PrepareAddUserToStoreSearchModel(new AddUserToStoreSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserAddPopupList(AddUserToStoreSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedKendoGridJson();

            var model = await _storeModelFactory.PrepareAddUserToStoreListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> UserAddPopup(AddUserToStoreModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUserStore))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(model.StoreId) ??
                        throw new ArgumentException("No store found with the specified id");

            foreach (var id in model.SelectedUserIds)
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    continue;
                
                if (user.UserStores.Count(mapping => mapping.StoreId == store.P_BranchNo) == 0)
                    user.UserStores.Add(new UserStore {Store = store});

                await _userService.UpdateUserAsync(user);
            }

            ViewBag.RefreshPage = true;

            return View(new AddUserToStoreSearchModel());
        }

        #endregion
    }
}