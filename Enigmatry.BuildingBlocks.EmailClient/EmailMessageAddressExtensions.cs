using MimeKit;

namespace Enigmatry.BuildingBlocks.Email
{
    internal static class EmailMessageAddressExtensions
    {
        public static MailboxAddress ToMailboxAddress(this EmailMessageAddress address) =>
            new(address.Name, address.Address);
    }
}
