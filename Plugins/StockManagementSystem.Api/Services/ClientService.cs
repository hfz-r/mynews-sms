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

        public async Task<IList<ClientModel>> GetAllClientsAsync()
        {
            IQueryable<Client> query = _configurationDbContext.Clients
                .Include(y => y.ClientSecrets)
                .Include(z => z.RedirectUris);

            IList<Client> clients = await query.ToListAsync();
            IList<ClientModel> clientConfigurationModels = clients.Select(client => client.ToApiModel()).ToList();

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
                // Needed to be able to obtain refresh token.
                AllowOfflineAccess = true,
                AccessTokenLifetime = model.AccessTokenLifetime,
                AbsoluteRefreshTokenLifetime = model.RefreshTokenLifetime
            };

            AddOrUpdateClientSecret(client, model.ClientSecret);
            AddOrUpdateClientRedirectUrl(client, model.RedirectUrl);

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

            await _configurationDbContext.Clients.AddAsync(client);
            await _configurationDbContext.SaveChangesAsync();

            return client.Id;
        }

        public async Task UpdateClientAsync(ClientModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var currentClient = _configurationDbContext.Clients
                .Include(client => client.ClientSecrets)
                .Include(client => client.RedirectUris)
                .FirstOrDefault(client => client.Id == model.Id);

            if (currentClient == null)
                throw new ArgumentNullException(nameof(currentClient));

            AddOrUpdateClientSecret(currentClient, model.ClientSecret);
            AddOrUpdateClientRedirectUrl(currentClient, model.RedirectUrl);

            currentClient.ClientId = model.ClientId;
            currentClient.ClientName = model.ClientName;
            currentClient.Enabled = model.Enabled;
            currentClient.AccessTokenLifetime = model.AccessTokenLifetime;
            currentClient.AbsoluteRefreshTokenLifetime = model.RefreshTokenLifetime;

            _configurationDbContext.Clients.Update(currentClient);
            await _configurationDbContext.SaveChangesAsync();
        }

        public async Task<ClientModel> FindClientByIdAsync(int id)
        {
            var currentClient = await _configurationDbContext.Clients
                .Include(client => client.ClientSecrets)
                .Include(client => client.RedirectUris)
                .FirstOrDefaultAsync(client => client.Id == id);

            return currentClient?.ToApiModel();
        }

        public async Task<ClientModel> FindClientByClientIdAsync(string clientId)
        {
            var currentClient = await _configurationDbContext.Clients
                .Include(client => client.ClientSecrets)
                .Include(client => client.RedirectUris)
                .FirstOrDefaultAsync(client => client.ClientId == clientId);

            return currentClient?.ToApiModel();
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _configurationDbContext.Clients
                .Include(entity => entity.ClientSecrets)
                .Include(entity => entity.RedirectUris)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (client != null)
            {
                _configurationDbContext.Clients.Remove(client);
                await _configurationDbContext.SaveChangesAsync();
            }
        }

        private static void AddOrUpdateClientRedirectUrl(Client currentClient, string modelRedirectUrl)
        {
            // Ensure the client redirect url collection is not null
            if (currentClient.RedirectUris == null)
                currentClient.RedirectUris = new List<ClientRedirectUri>();

            var currentClientRedirectUri = currentClient.RedirectUris.FirstOrDefault();
            // Add new redirectUri
            if ((currentClientRedirectUri != null && currentClientRedirectUri.RedirectUri != modelRedirectUrl) ||
                currentClientRedirectUri == null)
            {
                // Remove all redirect uris as we may have only one.
                currentClient.RedirectUris.Clear();

                currentClient.RedirectUris.Add(new ClientRedirectUri
                {
                    Client = currentClient,
                    RedirectUri = modelRedirectUrl
                });
            }
        }

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
    }
}