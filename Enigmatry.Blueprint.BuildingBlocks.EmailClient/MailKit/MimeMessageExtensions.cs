using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
using MimeKit;

namespace Enigmatry.Blueprint.BuildingBlocks.Email.MailKit
{
    internal static class MimeMessageExtensions
    {
        internal static void SetEmailData(this MimeMessage message, EmailMessage email, SmtpSettings settings)
        {
            if (email.To.Count != 1)
            {
                throw new InvalidOperationException($"'{nameof(email.To)}' is allowed to contain only one email address");
            }

            if (email.From != null)
            {
                message.From.Add((MailboxAddress)email.From);
            }
            else
            {
                message.From.TryToAdd(settings.From);
            }

            message.To.AddRange(email.To.Select(address => (MailboxAddress)address));
            message.Cc.AddRange(email.Cc.Select(x => new MailboxAddress(Encoding.UTF8, x.DisplayName, x.Address)));

            message.Subject = email.Subject;

            var builder = new BodyBuilder
            {
                TextBody = GetPlainText(email.Content),
                HtmlBody = email.Content
            };
            message.Body = builder.ToMessageBody();
        }

        private static string GetPlainText(string html)
        {
            var body = Regex.Match(html, "<body.*?>(.*?)</body>", RegexOptions.Singleline).Value;
            return Regex.Replace(body, @"<(.|\n)*?>", "").Trim();
        }
    }
}
