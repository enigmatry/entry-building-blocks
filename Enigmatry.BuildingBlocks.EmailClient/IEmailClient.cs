using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.Email
{
    public interface IEmailClient
    {
        Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
        Task SendBulkAsync(IEnumerable<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
    }
}
