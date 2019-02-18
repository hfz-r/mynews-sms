using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Api.Models.ApiSettings.Clients;

namespace StockManagementSystem.Api.Services
{
    public interface IClientService
    {
        Task DeleteClientAsync(int id);

        Task<ClientModel> FindClientByClientIdAsync(string clientId);

        Task<ClientModel> FindClientByIdAsync(int id);

        Task<IList<ClientModel>> GetAllClientsAsync();

        Task<int> InsertClientAsync(ClientModel model);

        Task UpdateClientAsync(ClientModel model);
    }
}