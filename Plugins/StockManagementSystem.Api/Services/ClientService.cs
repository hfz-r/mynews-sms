using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.ApiSettings.Clients;
using StockManagementSystem.Core;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace StockManagementSystem.Api.Services
{
    public class ClientService : IClientService
    {
        private readonly IConfigurationDbContext _configurationDbContext;

        public ClientService(IConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        #region Utilities

        private static void AddOrUpdateClientSecret(Client currentClient, string modelClientSecretDescription)
        {
            // Ensure the client secrets collection is not null
            if (currentClient.ClientSecrets == null)
                currentClient.ClientSecrets = new List<ClientSecret>();

            var currentClientSecret = currentClient.ClientSecrets.FirstOrDefault();
            // Add new secret
            if ((currentClientSecret != null && currentClientSecret.Description != modelClientSecretDescription) ||
                currentClientSecret == null)
            {
                // Remove all secrets as we may have only one valid.
                currentClient.ClientSecrets.Clear();

                currentClient.ClientSecrets.Add(new ClientSecret
                {
                    Client = currentClient,
                    Value = modelClientSecretDescription.Sha256(),
                    Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret,
                    Description = modelClientSecretDescription
                });
            }
        }

        private static void SetJavaScriptBasedClient(Client client)
        {
            client.AllowedGrantTypes.Clear();
            client.AllowedGrantTypes = new List<ClientGrantType>
            {
                new ClientGrantType
                {
                    Client = client,
                    GrantType = OidcConstants.GrantTypes.Implicit
                }
            };
        }

        private static void SetMvcBasedClient(Client client)
        {
            client.AllowedGrantTypes.Clear();
            client.AllowedGrantTypes = new List<ClientGrantType>
            {
                new ClientGrantType
                {
                    Client = client,
                    GrantType = OidcConstants.GrantTypes.AuthorizationCode
                },
                new ClientGrantType
                {
                    Client = client,
                    GrantType = OidcConstants.GrantTypes.RefreshToken
                },
                new ClientGrantType
                {
                    Client = client,
                    GrantType = OidcConstants.GrantTypes.JwtBearer
                }
            };
        }

        #endregion

        public async Task<IList<ClientModel>> GetAllClientsAsync()
        {
            IQueryable<Client> query = _configurationDbContext.Clients
                .Include(z => z.ClientSecrets)
                .Include(y => y.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(w => w.AllowedCorsOrigins);

            IList<ClientModel> clientConfigurationModels = await query.Select(client => client.ToModel()).ToListAsync();

            return clientConfigurationModels;
        }

        public async Task<int> InsertClientAsync(ClientModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var client = new Client
            {
                ClientId = model.ClientId,
                ClientName = model.ClientName,
                Enabled = model.Enabled,
                AllowOfflineAccess = true,
                AccessTokenLifetime = model.AccessTokenLifetime,
                AbsoluteRefreshTokenLifetime = model.RefreshTokenLifetime,
                AccessTokenType = (int)AccessTokenType.Reference,
                UpdateAccessTokenClaimsOnRefresh = true,
                RequireConsent = false,
            };

            AddOrUpdateClientSecret(client, model.ClientSecret);

            if (model.JavaScriptClient)
                SetJavaScriptBasedClient(client);
            else
                SetMvcBasedClient(client);

            client.AllowedScopes = new List<ClientScope>
            {
                new ClientScope
                {
                    Client = client,
                    Scope = "sms_api"
                }
            };

            client.Claims = new List<ClientClaim>
            {
                new ClientClaim
                {
                    Client = client,
                    Type = JwtClaimTypes.Subject,
                    Value = client.ClientId
                },
                new ClientClaim
                {
                    Client = client,
                    Type = JwtClaimTypes.Name,
                    Value = client.ClientName
                }
            };

            try
            {
                await _configurationDbContext.Clients.AddAsync(client);
                await _configurationDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new DefaultException(e.Message);
            }

            return client.Id;
        }

        public async Task UpdateClientInfo(ClientModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var currentClient = _configurationDbContext.Clients
                .Include(client => client.ClientSecrets)
                .Include(client => client.AllowedGrantTypes)
                .FirstOrDefault(client => client.Id == model.Id);

            if (currentClient == null)
                throw new ArgumentNullException(nameof(currentClient));

            AddOrUpdateClientSecret(currentClient, model.ClientSecret);

            currentClient.ClientId = model.ClientId;
            currentClient.ClientName = model.ClientName;
            currentClient.Enabled = model.Enabled;
            currentClient.AccessTokenLifetime = model.AccessTokenLifetime;
            currentClient.AbsoluteRefreshTokenLifetime = model.RefreshTokenLifetime;

            if (model.JavaScriptClient)
                SetJavaScriptBasedClient(currentClient);
            else
                SetMvcBasedClient(currentClient);

            try
            {
                _configurationDbContext.Clients.Update(currentClient);
                await _configurationDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new DefaultException($"The client with (ClientId = {model.ClientId}) existed");
            }
            catch (Exception ex)
            {
                throw new DefaultException(ex.Message);
            }
        }

        public async Task<Client> FindClientByIdAsync(int id)
        {
            var currentClient = await _configurationDbContext.Clients
                .Include(client => client.AllowedGrantTypes)
                .Include(client => client.ClientSecrets)
                .Include(client => client.RedirectUris)
                .Include(client => client.PostLogoutRedirectUris)
                .Include(client => client.AllowedCorsOrigins)
                .FirstOrDefaultAsync(client => client.Id == id);

            return currentClient;
        }

        public async Task<ClientModel> FindClientByClientIdAsync(string clientId)
        {
            var currentClient = await _configurationDbContext.Clients
                .Include(client => client.ClientSecrets)
                .Include(client => client.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(w => w.AllowedCorsOrigins)
                .FirstOrDefaultAsync(client => client.ClientId == clientId);

            return currentClient?.ToModel();
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _configurationDbContext.Clients
                .Include(entity => entity.ClientSecrets)
                .Include(entity => entity.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(w => w.AllowedCorsOrigins)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (client != null)
            {
                _configurationDbContext.Clients.Remove(client);
                await _configurationDbContext.SaveChangesAsync();
            }
        }

        public async Task<IPagedList<ClientRedirectUri>> GetRedirectUris(int clientId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var client = await FindClientByIdAsync(clientId);

            return await Task.FromResult<IPagedList<ClientRedirectUri>>(
                new PagedList<ClientRedirectUri>(client.RedirectUris, pageIndex, pageSize));
        }

        public async Task<IPagedList<ClientPostLogoutRedirectUri>> GetPostLogoutUris(int clientId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var client = await FindClientByIdAsync(clientId);

            return await Task.FromResult<IPagedList<ClientPostLogoutRedirectUri>>(
                new PagedList<ClientPostLogoutRedirectUri>(
                    client.PostLogoutRedirectUris, pageIndex, pageSize));
        }

        public async Task<IPagedList<ClientCorsOrigin>> GetCostOriginsUris(int clientId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var client = await FindClientByIdAsync(clientId);

            return await Task.FromResult<IPagedList<ClientCorsOrigin>>(
                new PagedList<ClientCorsOrigin>(client.AllowedCorsOrigins, pageIndex, pageSize));
        }

        public async Task UpdateClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            _configurationDbContext.Clients.Update(client);
            await _configurationDbContext.SaveChangesAsync();
        }
    }
}