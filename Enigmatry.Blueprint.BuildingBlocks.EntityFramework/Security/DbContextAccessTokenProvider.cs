using System;
using System.Threading.Tasks;
using Enigmatry.Blueprint.BuildingBlocks.Core.Helpers;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Blueprint.BuildingBlocks.EntityFramework.Security
{
    public class DbContextAccessTokenProvider : IDbContextAccessTokenProvider
    {
        private readonly ILogger<DbContextAccessTokenProvider> _logger;

        public DbContextAccessTokenProvider(ILogger<DbContextAccessTokenProvider> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetAccessTokenAsync() => await GetTokenFromAzureServiceTokenProvider();

        private async Task<string> GetTokenFromAzureServiceTokenProvider()
        {
            try
            {
                //No need to cache the token or check for expiration
                // according to the docs this is all done automatically:
                // https://docs.microsoft.com/en-us/azure/key-vault/service-to-service-authentication#using-the-library
                var accessToken = await new AzureServiceTokenProvider()
                    .GetAccessTokenAsync("https://database.windows.net/");

                if (!accessToken.HasContent())
                {
                    _logger.LogWarning("Getting access token for managed service identity: Token is empty.");
                }
                return accessToken;
            }
            catch (Exception e)
            {
                _logger.LogError("Error when getting access token for managed service identity", e);
                throw;
            }
        }
    }
}
