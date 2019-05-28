using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Delta;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.DTOs.TransporterTransaction;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.ModelBinders;
using StockManagementSystem.Api.Models.TransactionsParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Transactions;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TransporterTransactionController : BaseApiController
    {
        private readonly ITransporterTransactionApiService _transporterTransactionApiService;
        private readonly IRepository<TransporterTransaction> _transporterTransactionRepository;

        public TransporterTransactionController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            IUserService userService,
            ITenantMappingService tenantMappingService,
            ITenantService tenantService,
            IUserActivityService userActivityService,
            ITransporterTransactionApiService transporterTransactionApiService,
            IRepository<TransporterTransaction> transporterTransactionRepository)
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _transporterTransactionApiService = transporterTransactionApiService;
            _transporterTransactionRepository = transporterTransactionRepository;
        }

        /// <summary>
        /// Retrieve all transporter transaction
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/transporter_transaction")]
        [ProducesResponseType(typeof(TransporterTransactionRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetTransporterTransaction(TransporterTransactionParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var transporterTransactionDto = _transporterTransactionApiService.GetTransporterTransaction(
                    parameters.Ids,
                    parameters.Limit,
                    parameters.Page,
                    parameters.SinceId,
                    parameters.CreatedAtMin,
                    parameters.CreatedAtMax)
                .Select(trans => trans.ToDto()).ToList();

            var rootObject = new TransporterTransactionRootObject { TransporterTransaction = transporterTransactionDto };

            var json = JsonFieldsSerializer.Serialize(rootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Get a count of all transporter transaction
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/transporter_transaction/count")]
        [ProducesResponseType(typeof(TransporterTransactionCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetTransporterTransactionCount(TransporterTransactionParametersModel parameters)
        {
            var transCount = _transporterTransactionApiService.GetTransporterTransactionCount(parameters.CreatedAtMin, parameters.CreatedAtMax);

            var countRootObject = new TransporterTransactionCountRootObject() { Count = transCount };

            return await Task.FromResult<IActionResult>(Ok(countRootObject));
        }

        /// <summary>
        /// Retrieve transporter transaction by id
        /// </summary>
        /// <param name="id">Id of the transporter transaction</param>
        /// <param name="fields">Fields from the transporter transaction you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/transporter_transaction/{id}")]
        [ProducesResponseType(typeof(TransporterTransactionRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetTransporterTransactionById(int id, string fields = "")
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var transporterTransaction = _transporterTransactionApiService.GetTransporterTransactionById(id);
            if (transporterTransaction == null)
                return await Error(HttpStatusCode.NotFound, "transporter_transaction", "not found");

            var rootObject = new TransporterTransactionRootObject();
            rootObject.TransporterTransaction.Add(transporterTransaction.ToDto());

            var json = JsonFieldsSerializer.Serialize(rootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Create a new transporter transaction
        /// </summary>
        [HttpPost]
        [Route("/api/transporter_transaction")]
        [ProducesResponseType(typeof(TransporterTransactionRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateTransporterTransaction([ModelBinder(typeof(JsonModelBinder<TransporterTransactionDto>))] Delta<TransporterTransactionDto> transDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var newTransporterTransaction = new TransporterTransaction();
            transDelta.Merge(newTransporterTransaction);

            await _transporterTransactionRepository.InsertAsync(newTransporterTransaction);

            await UserActivityService.InsertActivityAsync("AddNewTransporterTransaction", $"Added a new transporter transaction (ID = {newTransporterTransaction.Id})", newTransporterTransaction);

            var newTransporterTransactionDto = newTransporterTransaction.ToDto();

            var rootObject = new TransporterTransactionRootObject();
            rootObject.TransporterTransaction.Add(newTransporterTransactionDto);

            var json = JsonFieldsSerializer.Serialize(rootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Update transporter transaction by id
        /// </summary>
        [HttpPut]
        [Route("/api/transporter_transaction/{id}")]
        [ProducesResponseType(typeof(TransporterTransactionRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateTransporterTransaction([ModelBinder(typeof(JsonModelBinder<TransporterTransactionDto>))] Delta<TransporterTransactionDto> transDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var currentTransporterTransaction = _transporterTransactionApiService.GetTransporterTransactionById(transDelta.Dto.Id);
            if (currentTransporterTransaction == null)
                return await Error(HttpStatusCode.NotFound, "transporter_transaction", "not found");

            transDelta.Merge(currentTransporterTransaction);

            await _transporterTransactionRepository.UpdateAsync(currentTransporterTransaction);

            await UserActivityService.InsertActivityAsync("EditTransporterTransaction", $"Edited a transporter transaction (ID = {currentTransporterTransaction.Id})", currentTransporterTransaction);

            var transporterTransactionDto = currentTransporterTransaction.ToDto();

            var rootObject = new TransporterTransactionRootObject();
            rootObject.TransporterTransaction.Add(transporterTransactionDto);

            var json = JsonFieldsSerializer.Serialize(rootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Delete transporter transaction by id
        /// </summary>
        [HttpDelete]
        [Route("/api/transporter_transaction/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteTransporterTransaction(int id)
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "Invalid id");

            var transporterTransaction = _transporterTransactionApiService.GetTransporterTransactionById(id);
            if (transporterTransaction == null)
                return await Error(HttpStatusCode.NotFound, "transporter_transaction", "TransporterTransaction not found");

            await _transporterTransactionRepository.DeleteAsync(transporterTransaction);

            await UserActivityService.InsertActivityAsync("DeleteTransporterTransaction", $"Deleted a transporter transaction (ID = {id})", transporterTransaction);

            return new RawJsonActionResult("{}");
        }
    }
}