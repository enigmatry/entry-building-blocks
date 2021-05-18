using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.EntityFramework.Security
{
    public interface IDbContextAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync();
    }
}
