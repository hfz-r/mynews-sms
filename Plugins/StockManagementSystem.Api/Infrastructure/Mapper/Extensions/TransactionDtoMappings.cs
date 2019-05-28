using StockManagementSystem.Api.DTOs.Transactions;
using StockManagementSystem.Core.Domain.Transactions;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class TransactionDtoMappings
    {
        public static TransactionDto ToDto(this Transaction transaction)
        {
            return transaction.MapTo<Transaction, TransactionDto>();
        }
    }
}