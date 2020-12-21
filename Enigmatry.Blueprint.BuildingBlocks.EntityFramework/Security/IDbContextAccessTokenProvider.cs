using System.Threading.Tasks;

namespace Enigmatry.Blueprint.BuildingBlocks.Infrastructure.EntityFramework.Security
{
    public interface IDbContextAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync();
    }
}
