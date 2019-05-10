using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Delta;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.DTOs.PushNotifications;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.ModelBinders;
using StockManagementSystem.Api.Models.PushNotificationsParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.PushNotifications;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PushNotificationsController : BaseApiController
    {
        private readonly IPushNotificationApiService _pushNotificationApiService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IStoreService _storeService;

        public PushNotificationsController(
            IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService, 
            IUserService userService, 
            ITenantMappingService tenantMappingService, 
            ITenantService tenantService, 
            IUserActivityService userActivityService,
            IPushNotificationApiService pushNotificationApiService,
            IPushNotificationService pushNotificationService,
            IStoreService storeService) 
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _pushNotificationApiService = pushNotificationApiService;
            _pushNotificationService = pushNotificationService;
            _storeService = storeService;
        }

        /// <summary>
        /// Retrieve all push notifications
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/pushnotification")]
        [ProducesResponseType(typeof(PushNotificationRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetPushNotifications(PushNotificationsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            IList<PushNotificationDto> pushNotificationDto =
                _pushNotificationApiService.GetPushNotifications(
                        parameters.CreatedAtMin,
                        parameters.CreatedAtMax,
                        parameters.Limit,
                        parameters.Page,
                        parameters.SinceId,
                        parameters.StoreIds)
                    .Select(pn => pn.ToDto()).ToList();

            var rootObj = new PushNotificationRootObject {PushNotifications = pushNotificationDto};

            var json = JsonFieldsSerializer.Serialize(rootObj, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Get a count of all push notifications
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/pushnotification/count")]
        [ProducesResponseType(typeof(PushNotificationCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPushNotificationsCount()
        {
            var pnCount = _pushNotificationApiService.GetPushNotificationsCount();

            var countRootObj = new PushNotificationCountRootObject {Count = pnCount};

            return await Task.FromResult<IActionResult>(Ok(countRootObj));
        }

        /// <summary>
        /// Retrieve push notification by id
        /// </summary>
        /// <param name="id">Id of the push notification</param>
        /// <param name="fields">Fields from the push notification you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/pushnotification/{id}")]
        [ProducesResponseType(typeof(PushNotificationRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetPushNotificationById(int id, string fields = "")
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var pn = _pushNotificationApiService.GetPushNotificationById(id);
            if (pn == null)
                return await Error(HttpStatusCode.NotFound, "push_notification", "not found");

            var rootObj = new PushNotificationRootObject();
            rootObj.PushNotifications.Add(pn.ToDto());

            var json = JsonFieldsSerializer.Serialize(rootObj, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Create new push notification
        /// </summary>
        [HttpPost]
        [Route("/api/pushnotification")]
        [ProducesResponseType(typeof(PushNotificationRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreatePushNotification([ModelBinder(typeof(JsonModelBinder<PushNotificationDto>))] Delta<PushNotificationDto> pushNotificationDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var newPushNotification = new PushNotification();
            pushNotificationDelta.Merge(newPushNotification);

            await _pushNotificationService.InsertPushNotification(newPushNotification);

            //stores
            if (pushNotificationDelta.Dto.StoreIds.Count > 0)
            {
                await AddValidStores(pushNotificationDelta, newPushNotification);
                _pushNotificationService.UpdatePushNotification(newPushNotification);
            }

            await UserActivityService.InsertActivityAsync("AddNewPushNotification", $"Added a new push notification (ID = {newPushNotification.Id})", newPushNotification);

            var rootObj = new PushNotificationRootObject();
            rootObj.PushNotifications.Add(newPushNotification.ToDto());

            var json = JsonFieldsSerializer.Serialize(rootObj, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Update push notification by id
        /// </summary>
        [HttpPut]
        [Route("/api/pushnotification/{id}")]
        [ProducesResponseType(typeof(PushNotificationRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdatePushNotification([ModelBinder(typeof(JsonModelBinder<PushNotificationDto>))] Delta<PushNotificationDto> pushNotificationDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var currentPushNotification = _pushNotificationApiService.GetPushNotificationById(pushNotificationDelta.Dto.Id);
            if (currentPushNotification == null)
                return await Error(HttpStatusCode.NotFound, "push_notification", "not found");

            pushNotificationDelta.Merge(currentPushNotification);

            //stores
            if (pushNotificationDelta.Dto.StoreIds.Count > 0)
                await AddValidStores(pushNotificationDelta, currentPushNotification);

            _pushNotificationService.UpdatePushNotification(currentPushNotification);

            await UserActivityService.InsertActivityAsync("EditPushNotification", $"Edited a push notification (ID = {currentPushNotification.Id})", currentPushNotification);

            var rootObj = new PushNotificationRootObject();
            rootObj.PushNotifications.Add(currentPushNotification.ToDto());

            var json = JsonFieldsSerializer.Serialize(rootObj, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Delete push notification by id
        /// </summary>
        [HttpDelete]
        [Route("/api/pushnotification/{id}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeletePushNotification(int id)
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "Invalid id");

            var pushNotification = _pushNotificationApiService.GetPushNotificationById(id);
            if (pushNotification == null)
                return await Error(HttpStatusCode.NotFound, "push_notification", "not found");

            _pushNotificationService.DeletePushNotification(pushNotification);

            await UserActivityService.InsertActivityAsync("DeletePushNotification", $"Deleted a push notification (ID = {id})", pushNotification);

            return new RawJsonActionResult("{}");
        }

        #region Private methods

        private async Task AddValidStores(Delta<PushNotificationDto> pushNotificationDelta, PushNotification currentPushNotification)
        {
            var stores = await _storeService.GetStores();
            foreach (var store in stores)
            {
                if (pushNotificationDelta.Dto.StoreIds.Contains(store.P_BranchNo))
                {
                    if (currentPushNotification.PushNotificationStores.Count(mapping => mapping.StoreId == store.P_BranchNo) == 0)
                        currentPushNotification.PushNotificationStores.Add(new PushNotificationStore { Store = store });
                }
                else
                {
                    if (currentPushNotification.PushNotificationStores.Count(mapping => mapping.StoreId == store.P_BranchNo) > 0)
                        currentPushNotification.PushNotificationStores.Remove(currentPushNotification.PushNotificationStores.FirstOrDefault(mapping => mapping.StoreId == store.P_BranchNo));
                }
            }
        }

        #endregion
    }
}