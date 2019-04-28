using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Management;
using StockManagementSystem.Services.Management;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Controllers
{
    public class ManagementController : BaseController
    {
        private readonly IOutletManagementService _outletManagementService;
        private readonly IUserService _userService;
        private readonly IStoreService _storeService;
        private readonly IManagementModelFactory _managementModelFactory;
        private readonly IRepository<StoreUserAssign> _storeUserAssignRepository;
        private readonly IRepository<StoreUserAssignStores> _storeUserAssignStoresRepository;
        private readonly IRepository<StoreGrouping> _storeGroupingRepository;
        private readonly IRepository<StoreGroupingStores> _storeGroupingStoresRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        #region Constructor

        public ManagementController(
            IOutletManagementService outletManagementService,
            IUserService userService,
            IStoreService storeService,
            IManagementModelFactory managementModelFactory,
            IRepository<StoreUserAssign> storeUserAssignRepository,
            IRepository<StoreUserAssignStores> storeUserAssignStoresRepository,
            IRepository<StoreGrouping> storeGroupingRepository,
            IRepository<StoreGroupingStores> storeGroupingStoresRepository,
            IRepository<Store> storeRepository,
            IRepository<User> userRepository,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILoggerFactory loggerFactory)
        {
            this._outletManagementService = outletManagementService;
            this._userService = userService;
            this._storeService = storeService;
            this._managementModelFactory = managementModelFactory;
            this._storeUserAssignRepository = storeUserAssignRepository;
            this._storeUserAssignStoresRepository = storeUserAssignStoresRepository;
            this._storeGroupingRepository = storeGroupingRepository;
            this._storeGroupingStoresRepository = storeGroupingStoresRepository;
            this._storeRepository = storeRepository;
            this._userRepository = userRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _logger = loggerFactory.CreateLogger<ManagementController>();
        }

        public ILogger Logger { get; }

        #endregion

        #region Outlet Management

        public async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedView();

            var model = await _managementModelFactory.PrepareOutletManagementContainerModel(new OutletManagementContainerModel());

            return View(model);
        }

        #region Assign User

        [HttpPost]
        public async Task<IActionResult> AssignUserList(AssignUserSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedKendoGridJson();

            var model = await _managementModelFactory.PrepareAssignUserListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddAssignUser(AssignUserModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedView();

            //validate store
            if (model.SelectedUserStoreId == 0)
            {
                ModelState.AddModelError(string.Empty, "Store is required");
                _notificationService.ErrorNotification("Store is required");
            }

            try
            {
                //Store
                StoreUserAssign storeUserAssign = new StoreUserAssign
                {
                    StoreId = model.SelectedUserStoreId,
                    StoreUserAssignStore = new List<StoreUserAssignStores>()
                };

                //Add user
                foreach (var user in model.SelectedUserIds)
                {
                    StoreUserAssignStores storeUserAssignStores = new StoreUserAssignStores
                    {
                        StoreUserAssignId = storeUserAssign.Id,
                        UserId = user,
                    };

                    storeUserAssign.StoreUserAssignStore.Add(storeUserAssignStores);
                }

                await _outletManagementService.InsertAssignUser(storeUserAssign);

                return new NullJsonResult();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                _notificationService.ErrorNotification(e.Message);

                return Json(e.Message);
            }
        }

        public async Task<IActionResult> EditAssignUser(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedView();

            var assignUser = await _outletManagementService.GetAssignUserByIdAsync(id);
            if (assignUser == null)
                return RedirectToAction("AssignUserList");

            var model = await _managementModelFactory.PrepareAssignUserModel(null, assignUser);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> EditAssignUser(AssignUserModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedView();

            var assignUser = await _outletManagementService.GetAssignUserByIdAsync(model.Id);
            if (assignUser == null)
                return RedirectToAction("Index");

            //validate store users
            var allUsers = await _userService.GetUsersAsync();
            var newUsers = new List<User>();
            foreach (var user in allUsers)
            {
                if (model.SelectedUserIds.Contains(user.Id))
                    newUsers.Add(user);
            }

            if (model.SelectedUserIds.Count == 0)
            {
                _notificationService.ErrorNotification("User is required");
                model = await _managementModelFactory.PrepareAssignUserModel(model, assignUser);
                model.SelectedUserIds = new List<int>();

                return View(model);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    assignUser.StoreId = model.SelectedUserStoreId;

                    //users
                    List<StoreUserAssignStores> storeUserAssignStoresList = new List<StoreUserAssignStores>();

                    foreach (var user in allUsers)
                    {
                        if (model.SelectedUserIds.Contains(user.Id))
                        {
                            //new user
                            if (assignUser.StoreUserAssignStore.Count(mapping => mapping.UserId == user.Id) == 0)
                            {
                                StoreUserAssignStores storeUserAssignStores = new StoreUserAssignStores
                                {
                                    StoreUserAssignId = assignUser.StoreId,
                                    UserId = user.Id
                                };
                                assignUser.StoreUserAssignStore.Add(storeUserAssignStores);
                            }
                        }
                        else
                        {
                            //remove user
                            if (assignUser.StoreUserAssignStore.Count(mapping => mapping.UserId == user.Id) > 0)
                                _outletManagementService.DeleteAssignUserUsers(model.Id, user);
                        }
                    }
                    _outletManagementService.UpdateAssignUser(assignUser);

                    _notificationService.SuccessNotification("Assign users has been updated successfully.");

                    //set to active tab
                    SaveSelectedTabName("assignUserTab", true);

                    if (!continueEditing)
                        return RedirectToAction("Index");

                    return RedirectToAction("EditAssignUser", new { id = assignUser.Id });
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }
            }

            model = await _managementModelFactory.PrepareAssignUserModel(model, assignUser);

            return View(model);
        }

        public async Task<IActionResult> DeleteAssignUser(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedView();

            var assignUser = await _outletManagementService.GetAssignUserByIdAsync(id);
            if (assignUser == null)
                return RedirectToAction("AssignUserList");

            try
            {
                _outletManagementService.DeleteAssignUser(assignUser);

                _notificationService.SuccessNotification("Assign user has been deleted successfully.");

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return RedirectToAction("EditAssignUser", new { id = assignUser.Id });
            }
        }

        #endregion

        #region Group Outlet

        [HttpPost]
        public async Task<IActionResult> GroupOutletList(GroupOutletSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedKendoGridJson();

            var model = await _managementModelFactory.PrepareGroupOutletListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddGroupOutlet(GroupOutletModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedView();

            if (model.SelectedStoreIds.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Store is required");
                _notificationService.ErrorNotification("Store is required");
            }

            try
            {
                StoreGrouping storeGrouping = new StoreGrouping
                {
                    GroupName = model.GroupName,
                    Store = new List<Store>()
                };
                _storeGroupingRepository.Update(storeGrouping);

                //Update store grouping 
                var storeList = await _storeService.GetStores();

                foreach (var store in model.SelectedStoreIds)
                {
                    var stores = storeList.FirstOrDefault(s => s.P_BranchNo == store);
                    if (stores != null)
                        stores.StoreGroupingId = storeGrouping.Id;

                    await _storeService.UpdateStore(stores);
                }

                return new NullJsonResult();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                _notificationService.ErrorNotification(e.Message);

                return Json(e.Message);
            }
        }

        public async Task<IActionResult> EditGroupOutlet(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedView();

            var groupOutlet = await _outletManagementService.GetGroupOutletByIdAsync(id);
            if (groupOutlet == null)
                return RedirectToAction("GroupOutletList");

            var model = await _managementModelFactory.PrepareGroupOutletModel(null, groupOutlet);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> EditGroupOutlet(GroupOutletModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedView();

            var groupOutlet = await _outletManagementService.GetGroupOutletByIdAsync(model.Id);
            if (groupOutlet == null)
                return RedirectToAction("Index");

            //validate stores
            var allStores = await _storeService.GetStores();
            var newStores = new List<Store>();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                    newStores.Add(store);
            }

            if (model.SelectedStoreIds.Count == 0)
            {
                _notificationService.ErrorNotification("Store is required");
                model = await _managementModelFactory.PrepareGroupOutletModel(model, groupOutlet);
                model.SelectedStoreIds = new List<int>();

                return View(model);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    groupOutlet.GroupName = model.GroupName;

                    //stores
                    var storeList = await _storeService.GetStores();
                    foreach (var store in allStores)
                    {
                        if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                        {
                            //new store
                            if (groupOutlet.Store.Count(map => map.P_BranchNo == store.P_BranchNo) == 0)
                            {
                                var stores = storeList.FirstOrDefault(s => s.P_BranchNo == store.P_BranchNo);
                                if (stores != null)
                                    stores.StoreGroupingId = groupOutlet.Id;

                                await _storeService.UpdateStore(stores);
                            }
                        }
                        else
                        {
                            //remove store grouping id
                           if (groupOutlet.Store.Count(mapping => mapping.P_BranchNo == store.P_BranchNo) > 0)
                            _outletManagementService.DeleteStoreGroupingId(model.Id, store);
                        }
                    }

                    _outletManagementService.UpdateGroupOutlet(groupOutlet);

                    _notificationService.SuccessNotification("Store grouping has been updated successfully.");

                    //set to active tab
                    SaveSelectedTabName("groupOutletTab", true);

                    if (!continueEditing)
                        return RedirectToAction("Index");

                    return RedirectToAction("EditGroupOutlet", new { id = groupOutlet.Id });
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }
            }

            model = await _managementModelFactory.PrepareGroupOutletModel(model, groupOutlet);

            return View(model);
        }

        public async Task<IActionResult> DeleteGroupOutlet(int id)
        {
            int? value = null;

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOutletManagement))
                return AccessDeniedView();

            var groupOutlet = await _outletManagementService.GetGroupOutletByIdAsync(id);
            if (groupOutlet == null)
                return RedirectToAction("GroupOutletList");

            try
            {
                //Update store grouping 
                var storeList = await _storeService.GetStores();
                foreach (var store in groupOutlet.Store.ToList())
                {
                    var stores = storeList.FirstOrDefault(s => s.P_BranchNo == store.P_BranchNo);
                    if (stores != null)
                        stores.StoreGroupingId = value;

                    await _storeService.UpdateStore(stores);
                }

                _outletManagementService.DeleteGroupOutlet(groupOutlet);

                _notificationService.SuccessNotification("Store grouping has been deleted successfully.");

                //set to active tab
                SaveSelectedTabName("groupOutletTab", true);
               
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return RedirectToAction("EditGroupOutlet", new { id = groupOutlet.Id });
            }
        }
        #endregion

        #endregion
    }
}