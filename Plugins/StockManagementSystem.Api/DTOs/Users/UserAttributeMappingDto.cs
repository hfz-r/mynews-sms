using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.DTOs.Users
{
    public class UserAttributeMappingDto
    {
        public User User { get; set; }
        public GenericAttribute Attribute { get; set; }
    }
}