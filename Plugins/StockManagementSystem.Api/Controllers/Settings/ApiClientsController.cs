using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Models.ApiSettings.Clients;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc.Filters;

namespace StockManagementSystem.Api.Controllers.Settings
{
    [Route("ApiClients/")]
    public class ApiClientsController : BasePluginController
    {
        private readonly IApiSettingModelFactory _apiSettingModelFactory;
        private readonly IUserActivityService _userActivityService;
        private readonly INotificationService _notificationService;
        private readonly IClientService _clientService;
        private readonly IPermissionService _permissionService;

        public ApiClientsController(
            IApiSettingModelFactory apiSettingModelFactory, 
            IUserActivityService userActivityService, 
            INotificationService notificationService,
            IClientService clientService,
            IPermissionService permissionService)
        {
            _apiSettingModelFactory = apiSettingModelFactory;
            _userActivityService = userActivityService;
            _notificationService = notificationService;
            _clientService = clientService;
            _permissionService = permissionService;
        }

        protected async Task<string> ValidateClient(ClientModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var client = await _clientService.FindClientByIdAsync(model.Id);
            if (client != null)
            {
                if (client.ClientId != model.ClientId)
                {
                    var findClientId = await _clientService.FindClientByClientIdAsync(model.ClientId) != null;
                    if (findClientId)
                        return $"The client with (ClientID = {model.ClientId}) existed.";
                }
            }

            return string.Empty;
        }

        [Route("List")]
        public async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //select "customer form fields" tab
            SaveSelectedTabName("tab-clientsettings");

            return RedirectToAction("Index", "ApiSettings");
        }

        [HttpPost]
        [Route("List")]
        public async Task<IActionResult> List(ClientSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedKendoGridJson();

            var model = await _apiSettingModelFactory.PrepareApiClientListModel(searchModel);

            return Json(model);
        }

        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = new ClientModel
            {
                Enabled = true,
                ClientSecret = Guid.NewGuid().ToString(),
                ClientId = Guid.NewGuid().ToString(),
                AccessTokenLifetime = Configurations.DefaultAccessTokenExpiration,
                RefreshTokenLifetime = Configurations.DefaultRefreshTokenExpiration
            };

            return View(ViewNames.ApiClientsCreate, model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("Create")]
        public async Task<IActionResult> Create(ClientModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var clientError = await ValidateClient(model);
            if (!string.IsNullOrEmpty(clientError))
            {
                ModelState.AddModelError(string.Empty, clientError);
                _notificationService.ErrorNotification(clientError);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var clientId = await _clientService.InsertClientAsync(model);

                    //activity log
                    await _userActivityService.InsertActivityAsync("AddNewApiClient", $"Added a new api client (ID = {clientId})");

                    _notificationService.SuccessNotification("The new api client has been added successfully.");

                    if (!continueEditing)
                        return RedirectToAction("List");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = clientId });
                }
                catch (Exception ex)
                {
                    _notificationService.ErrorNotification(ex.Message);
                }
            }

            return RedirectToAction("List");
        }

        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(id);
            if (client == null)
                return RedirectToAction("List");

            return View(ViewNames.ApiClientsEdit, client);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(ClientModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var clientError = await ValidateClient(model);
            if (!string.IsNullOrEmpty(clientError))
            {
                ModelState.AddModelError(string.Empty, clientError);
                _notificationService.ErrorNotification(clientError);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _clientService.UpdateClientAsync(model);

                    //activity log
                    await _userActivityService.InsertActivityAsync("EditApiClient", $"Edited api client (ID = {model.Id})");

                    _notificationService.SuccessNotification("The api client has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("List");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = model.Id });
                }
                catch (Exception ex)
                {
                    _notificationService.ErrorNotification(ex.Message);
                }
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("Delete")]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            await _clientService.DeleteClientAsync(id);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteApiClient", $"Deleted a api client (ID = {id})");

            _notificationService.SuccessNotification("The api client has been deleted successfully.");

            return RedirectToAction("List");
        }
    }
}