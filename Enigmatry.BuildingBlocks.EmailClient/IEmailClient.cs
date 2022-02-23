using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.Email
{
    public interface IEmailClient
    {
        Task<EmailMessageSendResult> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailMessageSendResult>> SendBulkAsync(IEnumerable<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
    }
}
