using System.Threading.Tasks;

namespace Enigmatry.Blueprint.BuildingBlocks.Email
{
    public interface IEmailClient
    {
        Task SendAsync(EmailMessage email);
    }
}
