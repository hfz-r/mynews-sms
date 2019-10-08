using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Models.ApiSettings.Clients;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc;
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

            var client = await _clientService.FindClientByClientIdAsync(model.ClientId) != null;
            return client ? $"The client with (ClientID = {model.ClientId}) existed." : string.Empty;
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

            var model = await _apiSettingModelFactory.PrepareClientModel(new ClientModel(), null);

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
                    await _userActivityService.InsertActivityAsync("AddNewApiClient",
                        $"Added a new api client (ID = {clientId})");

                    _notificationService.SuccessNotification("The new api client has been added successfully.");

                    if (!continueEditing)
                        return RedirectToAction("List");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = clientId});
                }
                catch (Exception ex)
                {
                    _notificationService.ErrorNotification(ex.Message);
                }
            }

            //prepare model
            model = await _apiSettingModelFactory.PrepareClientModel(model, null, true);

            return View(ViewNames.ApiClientsCreate, model);
        }

        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(id);
            if (client == null)
                return RedirectToAction("List");

            var model = await _apiSettingModelFactory.PrepareClientModel(null, client);

            return View(ViewNames.ApiClientsEdit, model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(ClientModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(model.Id);
            if (client == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                try
                {
                    await _clientService.UpdateClientInfo(model);

                    //activity log
                    await _userActivityService.InsertActivityAsync("EditApiClient",
                        $"Edited api client (ID = {model.Id})");

                    _notificationService.SuccessNotification("The api client has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("List");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = model.Id});
                }
                catch (Exception ex)
                {
                    _notificationService.ErrorNotification(ex.Message);
                }
            }

            model = await _apiSettingModelFactory.PrepareClientModel(model, client, true);

            return View(ViewNames.ApiClientsEdit, model);
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

        #region Redirect uris

        [HttpPost]
        [Route("RedirectUrisList")]
        public async Task<IActionResult> RedirectUrisList(UrisSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedKendoGridJson();

            var client = await _clientService.FindClientByIdAsync(searchModel.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            var model = await _apiSettingModelFactory.PrepareRedirectUrisListModel(searchModel, client);

            return Json(model);
        }

        [HttpPost]
        [Route("AddRedirectUris")]
        public async Task<IActionResult> AddRedirectUris(RedirectUrisModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(model.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            client.RedirectUris.Add(new ClientRedirectUri
            {
                RedirectUri = model.Url
            });

            await _clientService.UpdateClient(client);

            return new NullJsonResult();
        }

        [HttpPost]
        [Route("UpdateRedirectUris")]
        public async Task<IActionResult> UpdateRedirectUris(RedirectUrisModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(model.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            var redirectUri = client.RedirectUris.Find(re => re.Id == model.Id);
            if (redirectUri.RedirectUri != model.Url)
                redirectUri.RedirectUri = model.Url;

            await _clientService.UpdateClient(client);

            return new NullJsonResult();
        }

        [HttpPost]
        [Route("RemoveRedirectUris")]
        public async Task<IActionResult> RemoveRedirectUris(RedirectUrisModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(model.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            var redirectUri = client.RedirectUris.Find(re => re.Id == model.Id);
            client.RedirectUris.Remove(redirectUri);

            await _clientService.UpdateClient(client);

            return new NullJsonResult();
        }

        #endregion

        #region Post-logout uris

        [HttpPost]
        [Route("PostLogoutList")]
        public async Task<IActionResult> PostLogoutList(UrisSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedKendoGridJson();

            var client = await _clientService.FindClientByIdAsync(searchModel.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            var model = await _apiSettingModelFactory.PreparePostLogoutListModel(searchModel, client);

            return Json(model);
        }

        [HttpPost]
        [Route("AddPostLogout")]
        public async Task<IActionResult> AddPostLogout(PostLogoutUrisModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(model.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            client.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri
            {
                PostLogoutRedirectUri = model.Url
            });

            await _clientService.UpdateClient(client);

            return new NullJsonResult();
        }

        [HttpPost]
        [Route("UpdatePostLogout")]
        public async Task<IActionResult> UpdatePostLogout(PostLogoutUrisModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(model.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            var postLogoutUri = client.PostLogoutRedirectUris.Find(re => re.Id == model.Id);
            if (postLogoutUri.PostLogoutRedirectUri != model.Url)
                postLogoutUri.PostLogoutRedirectUri = model.Url;

            await _clientService.UpdateClient(client);

            return new NullJsonResult();
        }

        [HttpPost]
        [Route("RemovePostLogout")]
        public async Task<IActionResult> RemovePostLogout(PostLogoutUrisModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(model.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            var postLogoutUri = client.PostLogoutRedirectUris.Find(re => re.Id == model.Id);
            client.PostLogoutRedirectUris.Remove(postLogoutUri);

            await _clientService.UpdateClient(client);

            return new NullJsonResult();
        }

        #endregion

        #region Cors origins uris

        [HttpPost]
        [Route("CorsOriginsList")]
        public async Task<IActionResult> CorsOriginsList(UrisSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedKendoGridJson();

            var client = await _clientService.FindClientByIdAsync(searchModel.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            var model = await _apiSettingModelFactory.PrepareCorsOriginsListModel(searchModel, client);

            return Json(model);
        }

        [HttpPost]
        [Route("AddCorsOrigins")]
        public async Task<IActionResult> AddCorsOrigins(CorsOriginUrisModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(model.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            client.AllowedCorsOrigins.Add(new ClientCorsOrigin
            {
                Origin = model.Url
            });

            await _clientService.UpdateClient(client);

            return new NullJsonResult();
        }

        [HttpPost]
        [Route("UpdateCorsOrigins")]
        public async Task<IActionResult> UpdateCorsOrigins(CorsOriginUrisModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(model.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            var corsOriginsUri = client.AllowedCorsOrigins.Find(re => re.Id == model.Id);
            if (corsOriginsUri.Origin != model.Url)
                corsOriginsUri.Origin = model.Url;

            await _clientService.UpdateClient(client);

            return new NullJsonResult();
        }

        [HttpPost]
        [Route("RemoveCorsOrigins")]
        public async Task<IActionResult> RemoveCorsOrigins(CorsOriginUrisModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var client = await _clientService.FindClientByIdAsync(model.ClientId) ?? throw new ArgumentException("No client found with the specified id");

            var corsOriginsUri = client.AllowedCorsOrigins.Find(re => re.Id == model.Id);
            client.AllowedCorsOrigins.Remove(corsOriginsUri);

            await _clientService.UpdateClient(client);

            return new NullJsonResult();
        }

        #endregion
    }
}