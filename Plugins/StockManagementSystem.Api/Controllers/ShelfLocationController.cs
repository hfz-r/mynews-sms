using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Delta;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.DTOs.ShelfLocation;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.ModelBinders;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Api.Models.ShelfLocationParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    public class ShelfLocationController : BaseApiController
    {
        private readonly IShelfLocationApiService _shelfLocationApiService;
        private readonly IRepository<ShelfLocation> _shelfLocationRepository;

        public ShelfLocationController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            IUserService userService,
            ITenantMappingService tenantMappingService,
            ITenantService tenantService,
            IUserActivityService userActivityService,
            IShelfLocationApiService shelfLocationApiService,
            IRepository<ShelfLocation> shelfLocationRepository)
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _shelfLocationApiService = shelfLocationApiService;
            _shelfLocationRepository = shelfLocationRepository;
        }

        #region Private methods

        protected async Task<IActionResult> CountRootObjectResult(int count)
        {
            var countRootObject = new ShelfLocationCountRootObject { Count = count > 0 ? count : 0 };

            return await Task.FromResult<IActionResult>(Ok(countRootObject));
        }

        protected async Task<IActionResult> RootObjectResult(IList<ShelfLocationDto> entities, string fields)
        {
            var rootObj = new ShelfLocationRootObject { ShelfLocation = entities };

            var json = JsonFieldsSerializer.Serialize(rootObj, fields);

            return await Task.FromResult<IActionResult>(new RawJsonActionResult(json));
        }

        #endregion

        /// <summary>
        /// Retrieve all shelf location
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shelf_location")]
        [ProducesResponseType(typeof(ShelfLocationRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetShelfLocation(ShelfLocationParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var shelfLocationDto = _shelfLocationApiService.GetShelfLocation(
                    parameters.Ids,
                    parameters.Limit,
                    parameters.Page,
                    parameters.SinceId,
                    parameters.CreatedAtMin,
                    parameters.CreatedAtMax)
                .Select(trans => trans.ToDto()).ToList();

            var rootObject = new ShelfLocationRootObject { ShelfLocation = shelfLocationDto };

            var json = JsonFieldsSerializer.Serialize(rootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Get a count of all shelf location
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shelf_location/count")]
        [ProducesResponseType(typeof(ShelfLocationCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetShelfLocationCount(ShelfLocationParametersModel parameters)
        {
            var shelfLocationCount = _shelfLocationApiService.GetShelfLocationCount(parameters.CreatedAtMin, parameters.CreatedAtMax);

            var countRootObject = new ShelfLocationCountRootObject() { Count = shelfLocationCount };

            return await Task.FromResult<IActionResult>(Ok(countRootObject));
        }

        /// <summary>
        /// Retrieve shelf location by id
        /// </summary>
        /// <param name="id">Id of the shelf location</param>
        /// <param name="fields">Fields from the shelf location you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shelf_location/{id}")]
        [ProducesResponseType(typeof(ShelfLocationRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetShelfLocationById(int id, string fields = "")
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var shelfLocation = _shelfLocationApiService.GetShelfLocationById(id);
            if (shelfLocation == null)
                return await Error(HttpStatusCode.NotFound, "shelf_location", "not found");

            var rootObject = new ShelfLocationRootObject();
            rootObject.ShelfLocation.Add(shelfLocation.ToDto());

            var json = JsonFieldsSerializer.Serialize(rootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Search for related shelf location matching supplied query
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shelf_location/search")]
        [ProducesResponseType(typeof(ShelfLocationRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Search(GenericSearchParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var entities = _shelfLocationApiService.Search(
                parameters.Query,
                parameters.Limit,
                parameters.Page,
                parameters.SortColumn,
                parameters.Descending,
                parameters.Count);

            return parameters.Count
                ? await CountRootObjectResult(entities.Count)
                : await RootObjectResult(entities.List, parameters.Fields);
        }

        /// <summary>
        /// Create a new shelf location
        /// </summary>
        [HttpPost]
        [Route("/api/shelf_location")]
        [ProducesResponseType(typeof(ShelfLocationRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateShelfLocation([ModelBinder(typeof(JsonModelBinder<ShelfLocationDto>))] Delta<ShelfLocationDto> shelfLocationDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var newShelfLocation = new ShelfLocation();
            shelfLocationDelta.Merge(newShelfLocation);

            await _shelfLocationRepository.InsertAsync(newShelfLocation);

            await UserActivityService.InsertActivityAsync("AddNewShelfLocation", $"Added a new shelf location (ID = {newShelfLocation.Id})", newShelfLocation);

            var newShelfLocationDto = newShelfLocation.ToDto();

            var rootObject = new ShelfLocationRootObject();
            rootObject.ShelfLocation.Add(newShelfLocationDto);

            var json = JsonFieldsSerializer.Serialize(rootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Update shelf location by id
        /// </summary>
        [HttpPut]
        [Route("/api/shelf_location/{id}")]
        [ProducesResponseType(typeof(ShelfLocationRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateShelfLocation([ModelBinder(typeof(JsonModelBinder<ShelfLocationDto>))] Delta<ShelfLocationDto> shelfLocationDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var currentShelfLocation = _shelfLocationApiService.GetShelfLocationById(shelfLocationDelta.Dto.Id);
            if (currentShelfLocation == null)
                return await Error(HttpStatusCode.NotFound, "shelf_location", "not found");

            shelfLocationDelta.Merge(currentShelfLocation);

            await _shelfLocationRepository.UpdateAsync(currentShelfLocation);

            await UserActivityService.InsertActivityAsync("EditShelfLocation", $"Edited a shelf location (ID = {currentShelfLocation.Id})", currentShelfLocation);

            var shelfLocationDto = currentShelfLocation.ToDto();

            var rootObject = new ShelfLocationRootObject();
            rootObject.ShelfLocation.Add(shelfLocationDto);

            var json = JsonFieldsSerializer.Serialize(rootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Delete shelf location by id
        /// </summary>
        [HttpDelete]
        [Route("/api/shelf_location/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteShelfLocation(int id)
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "Invalid id");

            var shelfLocation = _shelfLocationApiService.GetShelfLocationById(id);
            if (shelfLocation == null)
                return await Error(HttpStatusCode.NotFound, "shelf_location", "ShelfLocation not found");

            await _shelfLocationRepository.DeleteAsync(shelfLocation);

            await UserActivityService.InsertActivityAsync("DeleteShelfLocation", $"Deleted a shelf location (ID = {id})", shelfLocation);

            return new RawJsonActionResult("{}");
        }
    }
}