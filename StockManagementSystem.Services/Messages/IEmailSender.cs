using System.Threading.Tasks;

namespace StockManagementSystem.Services.Messages
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
