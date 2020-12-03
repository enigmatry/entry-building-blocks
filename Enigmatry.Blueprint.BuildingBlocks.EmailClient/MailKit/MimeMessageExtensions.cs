using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
using MimeKit;
using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Enigmatry.Blueprint.BuildingBlocks.Email.MailKit
{
    internal static class MimeMessageExtensions
    {
        public static void SetEmailData(this MimeMessage message, EmailMessage email, SmtpSettings settings)
        {
            message.To.AddRange(email.To.Select(address => (MailboxAddress)address));

            if (email.From != null)
            {
                AddFromAddress(message, email.From);
            }
            else
            {
                AddFromAddress(message, settings.From);
            }

            message.Subject = email.Subject;

            var builder = new BodyBuilder
            {
                TextBody = GetPlainText(email.Content),
                HtmlBody = email.Content
            };
            message.Body = builder.ToMessageBody();
        }

        private static void AddFromAddress(MimeMessage message, MailAddress fromAddress)
        {
            if (fromAddress != null)
            {
                message.From.Add((MailboxAddress)fromAddress);
            }
        }

        private static void AddFromAddress(MimeMessage message, string fromAddress)
        {
            if (!String.IsNullOrEmpty(fromAddress) && MailboxAddress.TryParse(fromAddress, out var mailboxAddress))
            {
                message.From.Add(mailboxAddress);
            }
        }

        private static string GetPlainText(string html)
        {
            var body = Regex.Match(html, "<body.*?>(.*?)</body>", RegexOptions.Singleline).Value;
            return Regex.Replace(body, @"<(.|\n)*?>", "").Trim();
        }
    }
}
