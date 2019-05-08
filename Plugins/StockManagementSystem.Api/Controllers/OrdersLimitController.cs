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
using StockManagementSystem.Api.DTOs.OrderLimit;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.ModelBinders;
using StockManagementSystem.Api.Models.OrdersLimitParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.OrderLimits;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersLimitController : BaseApiController
    {
        private readonly IOrderLimitApiService _orderLimitApiService;
        private readonly IOrderLimitService _orderLimitService;
        private readonly IStoreService _storeService;
        private readonly IFactory<OrderLimit> _factory;

        public OrdersLimitController(
            IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService, 
            IUserService userService, 
            ITenantMappingService tenantMappingService, 
            ITenantService tenantService, 
            IUserActivityService userActivityService,
            IOrderLimitApiService orderLimitApiService,
            IOrderLimitService orderLimitService,
            IStoreService storeService,
            IFactory<OrderLimit> factory) 
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _orderLimitApiService = orderLimitApiService;
            _orderLimitService = orderLimitService;
            _storeService = storeService;
            _factory = factory;
        }

        /// <summary>
        /// Retrieve all orders limit
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orderlimit")]
        [ProducesResponseType(typeof(OrderLimitRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrdersLimit(OrdersLimitParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            IList<OrderLimitDto> orderLimitDto =
                _orderLimitApiService.GetOrdersLimit(
                        parameters.CreatedAtMin,
                        parameters.CreatedAtMax,
                        parameters.Limit,
                        parameters.Page,
                        parameters.SinceId,
                        parameters.StoreIds)
                    .Select(ol => ol.ToDto()).ToList();

            var rootObj = new OrderLimitRootObject {OrdersLimit = orderLimitDto};

            var json = JsonFieldsSerializer.Serialize(rootObj, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Get a count of all orders limit
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orderlimit/count")]
        [ProducesResponseType(typeof(OrderLimitCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOrdersLimitCount()
        {
            var olCount = _orderLimitApiService.GetOrdersLimitCount();

            var countRootObj = new OrderLimitCountRootObject{Count = olCount};

            return await Task.FromResult<IActionResult>(Ok(countRootObj));
        }

        /// <summary>
        /// Retrieve order limit by id
        /// </summary>
        /// <param name="id">Id of the order limit</param>
        /// <param name="fields">Fields from the order limit you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orderlimit/{id}")]
        [ProducesResponseType(typeof(OrderLimitRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrderLimitById(int id, string fields = "")
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var ol = _orderLimitApiService.GetOrderLimitById(id);
            if (ol == null)
                return await Error(HttpStatusCode.NotFound, "order_limit", "not found");

            var rootObj = new OrderLimitRootObject();
            rootObj.OrdersLimit.Add(ol.ToDto());

            var json = JsonFieldsSerializer.Serialize(rootObj, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Create new order limit
        /// </summary>
        [HttpPost]
        [Route("/api/orderlimit")]
        [ProducesResponseType(typeof(OrderLimitRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateOrderLimit([ModelBinder(typeof(JsonModelBinder<OrderLimitDto>))] Delta<OrderLimitDto> orderLimitDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var newOrderLimit = _factory.Initialize();
            orderLimitDelta.Merge(newOrderLimit);

            await _orderLimitService.InsertOrderLimit(newOrderLimit);

            //stores
            if (orderLimitDelta.Dto.StoreIds.Count > 0)
            {
                await AddValidStores(orderLimitDelta, newOrderLimit);
                _orderLimitService.UpdateOrderLimit(newOrderLimit);
            }

            await UserActivityService.InsertActivityAsync("AddNewOrderLimit", $"Added a new order limit (ID = {newOrderLimit.Id})", newOrderLimit);

            var newOrderLimitDto = newOrderLimit.ToDto();

            var rootObj = new OrderLimitRootObject();
            rootObj.OrdersLimit.Add(newOrderLimitDto);

            var json = JsonFieldsSerializer.Serialize(rootObj, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Update order limit by id
        /// </summary>
        [HttpPut]
        [Route("/api/orderlimit/{id}")]
        [ProducesResponseType(typeof(OrderLimitRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateOrderLimit([ModelBinder(typeof(JsonModelBinder<OrderLimitDto>))] Delta<OrderLimitDto> orderLimitDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var currentOrderLimit = _orderLimitApiService.GetOrderLimitById(orderLimitDelta.Dto.Id);
            if (currentOrderLimit == null)
                return await Error(HttpStatusCode.NotFound, "order_limit", "not found");

            orderLimitDelta.Merge(currentOrderLimit);

            //stores
            if (orderLimitDelta.Dto.StoreIds.Count > 0)
                await AddValidStores(orderLimitDelta, currentOrderLimit);

            _orderLimitService.UpdateOrderLimit(currentOrderLimit);

            await UserActivityService.InsertActivityAsync("EditOrderLimit", $"Edited an order limit (ID = {currentOrderLimit.Id})", currentOrderLimit);

            var orderLimitDto = currentOrderLimit.ToDto();

            var rootObj = new OrderLimitRootObject();
            rootObj.OrdersLimit.Add(orderLimitDto);

            var json = JsonFieldsSerializer.Serialize(rootObj, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Delete order limit by id
        /// </summary>
        [HttpDelete]
        [Route("/api/orderlimit/{id}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteOrderLimit(int id)
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "Invalid id");

            var orderLimit = _orderLimitApiService.GetOrderLimitById(id);
            if (orderLimit == null)
                return await Error(HttpStatusCode.NotFound, "order_limit", "not found");

            _orderLimitService.DeleteOrderLimit(orderLimit);

            await UserActivityService.InsertActivityAsync("DeleteOrderLimit", $"Deleted an order limit (ID = {id})", orderLimit);

            return new RawJsonActionResult("{}");
        }

        #region Private methods

        private async Task AddValidStores(Delta<OrderLimitDto> orderLimitDelta, OrderLimit currentOrderLimit)
        {
            var stores = await _storeService.GetStores();
            foreach (var store in stores)
            {
                if (orderLimitDelta.Dto.StoreIds.Contains(store.P_BranchNo))
                {
                    if (currentOrderLimit.OrderLimitStores.Count(mapping => mapping.StoreId == store.P_BranchNo) == 0)
                        currentOrderLimit.OrderLimitStores.Add(new OrderLimitStore { Store = store });
                }
                else
                {
                    if (currentOrderLimit.OrderLimitStores.Count(mapping => mapping.StoreId == store.P_BranchNo) > 0)
                        currentOrderLimit.OrderLimitStores.Remove(currentOrderLimit.OrderLimitStores.FirstOrDefault(mapping => mapping.StoreId == store.P_BranchNo));
                }
            }
        }

        #endregion
    }
}