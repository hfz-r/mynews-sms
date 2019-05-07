using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Delta;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.DTOs.Items;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.ModelBinders;
using StockManagementSystem.Api.Models.ItemsParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ItemsController : BaseApiController
    {
        private readonly IItemApiService _itemApiService;
        private readonly IRepository<Item> _itemRepository;

        public ItemsController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            IUserService userService,
            ITenantMappingService tenantMappingService,
            ITenantService tenantService,
            IUserActivityService userActivityService,
            IItemApiService itemApiService,
            IRepository<Item> itemRepository)
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _itemApiService = itemApiService;
            _itemRepository = itemRepository;
        }

        /// <summary>
        /// Retrieve all items
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/items")]
        [ProducesResponseType(typeof(ItemsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetItems(ItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var itemsDto = _itemApiService.GetItems(
                parameters.Limit, 
                parameters.Page, 
                parameters.SinceId).Select(item => item.ToDto()).ToList();

            var itemsRootObject = new ItemsRootObject {Items = itemsDto};

            var json = JsonFieldsSerializer.Serialize(itemsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Get a count of all items
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/items/count")]
        [ProducesResponseType(typeof(ItemsCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetItemsCount()
        {
            var itemsCount = _itemApiService.GetItemsCount();

            var itemsCountRootObject = new ItemsCountRootObject {Count = itemsCount};

            return await Task.FromResult<IActionResult>(Ok(itemsCountRootObject));
        }

        /// <summary>
        /// Retrieve item by id
        /// </summary>
        /// <param name="id">Id of the item</param>
        /// <param name="fields">Fields from the item you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/items/{id}")]
        [ProducesResponseType(typeof(ItemsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetItemById(int id, string fields = "")
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var item = _itemApiService.GetItemById(id);
            if (item == null)
                return await Error(HttpStatusCode.NotFound, "item", "not found");

            var itemsRootObject = new ItemsRootObject();
            itemsRootObject.Items.Add(item.ToDto());

            var json = JsonFieldsSerializer.Serialize(itemsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Create a new item
        /// </summary>
        [HttpPost]
        [Route("/api/items")]
        [ProducesResponseType(typeof(ItemsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateItem([ModelBinder(typeof(JsonModelBinder<ItemDto>))] Delta<ItemDto> itemDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var newItem = new Item();
            itemDelta.Merge(newItem);

            await _itemRepository.InsertAsync(newItem);

            //activity log
            await UserActivityService.InsertActivityAsync("AddNewItem", $"Added a new item (ID = {newItem.Id})", newItem);

            var newItemDto = newItem.ToDto();

            var itemsRootObject = new ItemsRootObject();
            itemsRootObject.Items.Add(newItemDto);

            var json = JsonFieldsSerializer.Serialize(itemsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Update item by id
        /// </summary>
        [HttpPut]
        [Route("/api/items/{id}")]
        [ProducesResponseType(typeof(ItemsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateItem([ModelBinder(typeof(JsonModelBinder<ItemDto>))] Delta<ItemDto> itemDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var currentItem = _itemApiService.GetItemById(itemDelta.Dto.Id);
            if (currentItem == null)
                return await Error(HttpStatusCode.NotFound, "item", "not found");

            itemDelta.Merge(currentItem);

            await _itemRepository.UpdateAsync(currentItem);

            //activity log
            await UserActivityService.InsertActivityAsync("EditItem", $"Edited a item (ID = {currentItem.Id})", currentItem);

            var itemDto = currentItem.ToDto();

            var itemsRootObject = new ItemsRootObject();
            itemsRootObject.Items.Add(itemDto);

            var json = JsonFieldsSerializer.Serialize(itemsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Delete item by id
        /// </summary>
        [HttpDelete]
        [Route("/api/items/{id}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "Invalid id");

            var item = _itemApiService.GetItemById(id);
            if (item == null)
                return await Error(HttpStatusCode.NotFound, "item", "Item not found");

            await _itemRepository.DeleteAsync(item);

            //activity log
            await UserActivityService.InsertActivityAsync("DeleteItem", $"Deleted a item (ID = {id})", item);

            return new RawJsonActionResult("{}");
        }
    }
}