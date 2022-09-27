using System.Threading.Tasks;

namespace Enigmatry.Entry.EntityFramework.Security
{
    public interface IDbContextAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync();
    }
}
