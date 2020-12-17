using System;
using System.Threading.Tasks;

namespace Enigmatry.Blueprint.BuildingBlocks.Infrastructure.EntityFramework.Security
{
    public class NullDbContextAccessTokenProvider : IDbContextAccessTokenProvider
    {
        public Task<string> GetAccessTokenAsync() => Task.FromResult(String.Empty);
    }
}
