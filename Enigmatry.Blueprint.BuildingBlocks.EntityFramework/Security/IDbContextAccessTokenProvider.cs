using System.Threading.Tasks;

namespace Enigmatry.Blueprint.BuildingBlocks.EntityFramework.Security
{
    public interface IDbContextAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync();
    }
}
