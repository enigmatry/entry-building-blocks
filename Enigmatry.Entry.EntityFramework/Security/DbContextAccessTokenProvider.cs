using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Enigmatry.Entry.Core.Helpers;
using Enigmatry.Entry.Core.Settings;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.EntityFramework.Security;

public class DbContextAccessTokenProvider : IDbContextAccessTokenProvider
{
    private readonly DbContextSettings _settings;
    private readonly ILogger<DbContextAccessTokenProvider> _logger;

    public DbContextAccessTokenProvider(DbContextSettings settings, ILogger<DbContextAccessTokenProvider> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        return _settings.UseAccessToken ?
            await GetTokenFromAzureServiceTokenProvider() :
            await Task.FromResult(String.Empty);
    }

    private async Task<string> GetTokenFromAzureServiceTokenProvider()
    {
        try
        {
            var tokenCredential = new DefaultAzureCredential();
            var accessToken = await tokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new string[] { "https://database.windows.net/.default" }) { }
            );

            if (!accessToken.Token.HasContent())
            {
                _logger.LogWarning("Getting access token for managed service identity: Token is empty.");
            }
            return accessToken.Token;
        }
        catch (Exception e)
        {
            _logger.LogError("Error when getting access token for managed service identity", e);
            throw;
        }

    }
}
