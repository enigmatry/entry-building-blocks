using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Enigmatry.BuildingBlocks.Core.Helpers;
using Enigmatry.BuildingBlocks.Core.Settings;
using MimeKit;

namespace Enigmatry.BuildingBlocks.Email.MailKit
{
    internal static class MimeMessageExtensions
    {
        internal static void SetEmailData(this MimeMessage message, EmailMessage email, SmtpSettings settings)
        {
            message.MessageId = email.MessageId;

            if (email.To.Count != 1)
            {
                throw new InvalidOperationException(
                    $"'{nameof(email.To)}' is allowed to contain only one email address");
            }

            if (EmailMessageAddress.Empty.Equals(email.From))
            {
                message.From.TryAdd(settings.From);
            }
            else
            {
                message.From.Add(email.From.ToMailboxAddress());
            }

            if (settings.CatchAllAddress.HasContent())
            {
                message.To.TryAdd(settings.CatchAllAddress);
            }
            else
            {
                message.To.AddRange(email.To.Select(address => address.ToMailboxAddress()));
                message.Cc.AddRange(email.Cc.Select(address => address.ToMailboxAddress()));
                message.Bcc.AddRange(email.Bcc.Select(address => address.ToMailboxAddress()));
            }

            message.Subject = email.Subject;

            var builder = new BodyBuilder
            {
                TextBody = GetPlainText(email.Body),
                HtmlBody = email.Body
            };
            email.Attachments.ForEach(attachment => builder.Attachments.Add(attachment.FileName, new MemoryStream(attachment.Data), ContentType.Parse(attachment.ContentType)));
            message.Body = builder.ToMessageBody();
        }

        private static string GetPlainText(string html)
        {
            var body = Regex.Match(html, "<body.*?>(.*?)</body>", RegexOptions.Singleline).Value;
            return Regex.Replace(body, @"<(.|\n)*?>", "").Trim();
        }
    }
}
