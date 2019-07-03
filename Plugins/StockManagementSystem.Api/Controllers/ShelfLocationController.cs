using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        /// Retrieve shelf location by query attributes
        /// </summary>
        /// <param name="id">Shelf location id in <see cref="NameValueCollection"/></param>
        /// <param name="branchno">Branch no in <see cref="NameValueCollection"/></param>
        /// <param name="parameters">Additional filters to get specified result</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shelf_location/get")]
        [ProducesResponseType(typeof(ShelfLocationRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetShelfLocationById([FromQuery] int id, [FromQuery] int branchno, ShelfLocationParametersModel parameters)
        {
            ShelfLocationRootObject rootObject;

            if (id > 0)
            {
                var shelf = _shelfLocationApiService.GetShelfLocationById(id);
                if (shelf == null)
                    return await Error(HttpStatusCode.NotFound, "shelf_location", "not found");

                rootObject = new ShelfLocationRootObject();
                rootObject.ShelfLocation.Add(shelf.ToDto());

            }
            else if (branchno > 0)
            {
                IList<ShelfLocationDto> dtos = _shelfLocationApiService
                    .GetShelfLocationByBranchNo(branchno, parameters.CreatedAtMin, parameters.CreatedAtMax)
                    .Select(shelf => shelf.ToDto()).ToList();
                if (!dtos.Any())
                    return await Error(HttpStatusCode.NotFound, "shelf_location", "not found");

                rootObject = new ShelfLocationRootObject {ShelfLocation = dtos};
            }
            else
                return await Error(HttpStatusCode.BadRequest, "invalid", "invalid id or branch_no");

            var json = JsonFieldsSerializer.Serialize(rootObject, parameters.Fields);

            return new RawJsonActionResult(json);
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