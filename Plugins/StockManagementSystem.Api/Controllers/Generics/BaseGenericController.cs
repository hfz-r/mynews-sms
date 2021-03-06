﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.DTOs.Generics;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Contracts;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core;
using StockManagementSystem.Services.Logging;

namespace StockManagementSystem.Api.Controllers.Generics
{
    [Authorize(Policy = "ApiDefaultPolicy")]
    [Route("api/[controller]")]
    public class BaseGenericController<T, E> : Controller where T : BaseDto where E : BaseEntity
    {
        protected readonly IGenericApiService<T, E> GenericApiService;
        protected readonly IJsonFieldsSerializer JsonFieldsSerializer;
        protected readonly IUserActivityService UserActivityService;

        public BaseGenericController(
            IGenericApiService<T, E> genericApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IUserActivityService userActivityService)
        {
            JsonFieldsSerializer = jsonFieldsSerializer;
            UserActivityService = userActivityService;
            GenericApiService = genericApiService;
        }

        #region Protected methods

        protected async Task<IActionResult> Error(HttpStatusCode statusCode = (HttpStatusCode) 422,
            string propertyKey = "", string errorMessage = "")
        {
            var errors = new Dictionary<string, List<string>>();

            if (!string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(propertyKey))
            {
                var errorsList = new List<string> {errorMessage};
                errors.Add(propertyKey, errorsList);
            }

            foreach (var item in ModelState)
            {
                var errorMessages = item.Value.Errors.Select(x => x.ErrorMessage);

                var validErrorMessages = new List<string>();
                validErrorMessages.AddRange(errorMessages.Where(message => !string.IsNullOrEmpty(message)));

                if (validErrorMessages.Count > 0)
                {
                    if (errors.ContainsKey(item.Key))
                        errors[item.Key].AddRange(validErrorMessages);
                    else
                        errors.Add(item.Key, validErrorMessages.ToList());
                }
            }

            var errorsRootObject = new ErrorsRootObject {Errors = errors};

            var errorsJson = JsonFieldsSerializer.Serialize(errorsRootObject, null);

            return await Task.FromResult<IActionResult>(new ErrorActionResult(errorsJson, statusCode));
        }

        protected async Task<IActionResult> CountRootObjectResult(int count)
        {
            var rootObj = new GenericCountRootObject {Count = count > 0 ? count : 0};

            return await Task.FromResult<IActionResult>(Ok(rootObj));
        }

        protected async Task<IActionResult> RootObjectResult(IList<T> entities, string fields)
        {
            var rootObj = new GenericRootObject<T> {Entities = entities};

            var json = JsonFieldsSerializer.Serialize(rootObj, fields,
                new JsonSerializer {ContractResolver = new GenericTypeNameContractResolver()});

            return await Task.FromResult<IActionResult>(new RawJsonActionResult(json));
        }

        #endregion

        /// <summary>
        /// Retrieve all related entities
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ProducesResponseType(typeof(GenericRootObject<>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> Get(GenericsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var entities = await GenericApiService.GetAll(parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.SortColumn, parameters.Descending);

            var json = JsonFieldsSerializer.Serialize(new GenericRootObject<T> {Entities = entities}, parameters.Fields,
                new JsonSerializer {ContractResolver = new GenericTypeNameContractResolver()});

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve entity by id
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="fields">Specific from the entity to include in response</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GenericRootObject<>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> Get(int id, string fields = "")
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var entity = await GenericApiService.GetById(id);
            if (entity == null)
                return await Error(HttpStatusCode.NotFound, "entity", "not found");

            var rootObj = new GenericRootObject<T>();
            rootObj.Entities.Add(entity);

            var json = JsonFieldsSerializer.Serialize(rootObj, fields,
                new JsonSerializer {ContractResolver = new GenericTypeNameContractResolver()});

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Count entities
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("count")]
        [ProducesResponseType(typeof(GenericCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetEntitiesCount()
        {
            var count = await GenericApiService.Count();

            return Ok(new GenericCountRootObject { Count = count });
        }

        /// <summary>
        /// Search entity
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(GenericRootObject<>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> Search(GenericSearchParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var entities = await GenericApiService.Search(
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
    }
}