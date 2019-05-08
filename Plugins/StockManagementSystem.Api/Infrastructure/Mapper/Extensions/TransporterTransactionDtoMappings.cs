using StockManagementSystem.Api.DTOs.Generics;
using StockManagementSystem.Core.Domain.Transactions;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class TransporterTransactionDtoMappings
    {
        public static TransporterTransactionDto ToDto(this TransporterTransaction transporter)
        {
            return transporter.MapTo<TransporterTransaction, TransporterTransactionDto>();
        }
    }
}