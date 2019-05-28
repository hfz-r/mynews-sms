using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Delta;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.DTOs.Transactions;
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
    public class TransactionsController : BaseApiController
    {
        private readonly ITransactionApiService _transactionApiService;
        private readonly IRepository<Transaction> _transactionRepository;

        public TransactionsController(
            IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService,
            IUserService userService, 
            ITenantMappingService tenantMappingService, 
            ITenantService tenantService,
            IUserActivityService userActivityService,
            ITransactionApiService transactionApiService,
            IRepository<Transaction> transactionRepository) 
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _transactionApiService = transactionApiService;
            _transactionRepository = transactionRepository;
        }

        /// <summary>
        /// Retrieve all transactions
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/transactions")]
        [ProducesResponseType(typeof(TransactionsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetTransactions(TransactionsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var transactionDto = _transactionApiService.GetTransactions(
                    parameters.Ids,
                    parameters.Limit,
                    parameters.Page,
                    parameters.SinceId,
                    parameters.CreatedAtMin,
                    parameters.CreatedAtMax)
                .Select(trans => trans.ToDto()).ToList();

            var rootObject = new TransactionsRootObject { Transactions = transactionDto };

            var json = JsonFieldsSerializer.Serialize(rootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Get a count of all transactions
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/transactions/count")]
        [ProducesResponseType(typeof(TransactionsCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetTransactionsCount(TransactionsParametersModel parameters)
        {
            var transCount = _transactionApiService.GetTransactionsCount(parameters.CreatedAtMin, parameters.CreatedAtMax);

            var countRootObject = new TransactionsCountRootObject() { Count = transCount };

            return await Task.FromResult<IActionResult>(Ok(countRootObject));
        }

        /// <summary>
        /// Retrieve transaction by id
        /// </summary>
        /// <param name="id">Id of the transaction</param>
        /// <param name="fields">Fields from the transaction you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/transactions/{id}")]
        [ProducesResponseType(typeof(TransactionsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetTransactionById(int id, string fields = "")
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var transaction = _transactionApiService.GetTransactionById(id);
            if (transaction == null)
                return await Error(HttpStatusCode.NotFound, "transaction", "not found");

            var rootObject = new TransactionsRootObject();
            rootObject.Transactions.Add(transaction.ToDto());

            var json = JsonFieldsSerializer.Serialize(rootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Create a new transaction
        /// </summary>
        [HttpPost]
        [Route("/api/transactions")]
        [ProducesResponseType(typeof(TransactionsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateTransaction([ModelBinder(typeof(JsonModelBinder<TransactionDto>))] Delta<TransactionDto> transDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var newTransaction = new Transaction();
            transDelta.Merge(newTransaction);

            await _transactionRepository.InsertAsync(newTransaction);

            await UserActivityService.InsertActivityAsync("AddNewTransaction", $"Added a new transaction (ID = {newTransaction.Id})", newTransaction);

            var newTransactionDto = newTransaction.ToDto();

            var rootObject = new TransactionsRootObject();
            rootObject.Transactions.Add(newTransactionDto);

            var json = JsonFieldsSerializer.Serialize(rootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Update transaction by id
        /// </summary>
        [HttpPut]
        [Route("/api/transactions/{id}")]
        [ProducesResponseType(typeof(TransactionsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateTransaction([ModelBinder(typeof(JsonModelBinder<TransactionDto>))] Delta<TransactionDto> transDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var currentTransaction = _transactionApiService.GetTransactionById(transDelta.Dto.Id);
            if (currentTransaction == null)
                return await Error(HttpStatusCode.NotFound, "transaction", "not found");

            transDelta.Merge(currentTransaction);

            await _transactionRepository.UpdateAsync(currentTransaction);

            await UserActivityService.InsertActivityAsync("EditTransaction", $"Edited a transaction (ID = {currentTransaction.Id})", currentTransaction);

            var transactionDto = currentTransaction.ToDto();

            var rootObject = new TransactionsRootObject();
            rootObject.Transactions.Add(transactionDto);

            var json = JsonFieldsSerializer.Serialize(rootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Delete transaction by id
        /// </summary>
        [HttpDelete]
        [Route("/api/transactions/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "Invalid id");

            var transaction = _transactionApiService.GetTransactionById(id);
            if (transaction == null)
                return await Error(HttpStatusCode.NotFound, "transaction", "Transaction not found");

            await _transactionRepository.DeleteAsync(transaction);

            await UserActivityService.InsertActivityAsync("DeleteTransaction", $"Deleted a transaction (ID = {id})", transaction);

            return new RawJsonActionResult("{}");
        }
    }
}