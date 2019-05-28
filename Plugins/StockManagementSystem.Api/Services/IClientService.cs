using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using StockManagementSystem.Api.Models.ApiSettings.Clients;
using StockManagementSystem.Core;

namespace StockManagementSystem.Api.Services
{
    public interface IClientService
    {
        Task DeleteClientAsync(int id);

        Task<ClientModel> FindClientByClientIdAsync(string clientId);

        Task<Client> FindClientByIdAsync(int id);

        Task<IList<ClientModel>> GetAllClientsAsync();

        Task<int> InsertClientAsync(ClientModel model);

        Task UpdateClientInfo(ClientModel model);

        Task<IPagedList<ClientRedirectUri>> GetRedirectUris(int clientId, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<ClientPostLogoutRedirectUri>> GetPostLogoutUris(int clientId, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<ClientCorsOrigin>> GetCostOriginsUris(int clientId, int pageIndex = 0, int pageSize = int.MaxValue);

        Task UpdateClient(Client client);
    }
}