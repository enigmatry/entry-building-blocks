using MimeKit;

namespace Enigmatry.Entry.Email
{
    internal static class EmailMessageAddressExtensions
    {
        public static MailboxAddress ToMailboxAddress(this EmailMessageAddress address) =>
            new(address.Name, address.Address);
    }
}
