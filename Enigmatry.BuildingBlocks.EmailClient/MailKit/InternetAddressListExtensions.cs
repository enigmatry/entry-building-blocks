using System;
using MimeKit;

namespace Enigmatry.BuildingBlocks.Email.MailKit
{
    internal static class InternetAddressListExtensions
    {
        internal static void TryAdd(this InternetAddressList internetAddressList, string internetAddress)
        {
            var mailboxAddress = ParseMailAddress(internetAddress);
            if (mailboxAddress != null)
            {
                internetAddressList.Add(mailboxAddress);
            }
        }

        private static MailboxAddress? ParseMailAddress(string? mailAddress) =>
            !String.IsNullOrEmpty(mailAddress) && MailboxAddress.TryParse(mailAddress, out var mailboxAddress)
                ? mailboxAddress
                : null;
    }
}
