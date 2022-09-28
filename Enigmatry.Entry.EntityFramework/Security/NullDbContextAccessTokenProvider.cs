using System;
using System.Threading.Tasks;

namespace Enigmatry.Entry.EntityFramework.Security
{
    public class NullDbContextAccessTokenProvider : IDbContextAccessTokenProvider
    {
        public Task<string> GetAccessTokenAsync() => Task.FromResult(String.Empty);
    }
}
