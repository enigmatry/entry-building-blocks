using System;
using MimeKit;

namespace Enigmatry.Blueprint.BuildingBlocks.Email.MailKit
{
    internal static class InternetAddressListExtensions
    {
        internal static bool TryToAdd(this InternetAddressList internetAddressList, string internetAddress)
        {
            var mailboxAddress = ParseMailAddress(internetAddress);
            if (mailboxAddress == null)
            {
                return false;
            }

            internetAddressList.Add(mailboxAddress);
            return true;
        }

        private static MailboxAddress? ParseMailAddress(string? mailAddress) =>
            !String.IsNullOrEmpty(mailAddress) && MailboxAddress.TryParse(mailAddress, out var mailboxAddress)
                ? mailboxAddress
                : null;
    }
}
