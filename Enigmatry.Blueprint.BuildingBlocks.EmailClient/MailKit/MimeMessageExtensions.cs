using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
using MimeKit;
using MimeKit.Text;
using System;
using System.Linq;

namespace Enigmatry.Blueprint.BuildingBlocks.Email.MailKit
{
    internal static class MimeMessageExtensions
    {
        public static void SetEmailData(this MimeMessage message, EmailMessage email, SmtpSettings settings)
        {
            message.To.AddRange(email.To.Select(MailboxAddress.Parse));
            message.From.AddRange(email.From.Select(MailboxAddress.Parse));

            if (!String.IsNullOrEmpty(settings.From))
            {
                message.From.Add(MailboxAddress.Parse(settings.From));
            }

            message.Subject = email.Subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = email.Content
            };
        }
    }
}
