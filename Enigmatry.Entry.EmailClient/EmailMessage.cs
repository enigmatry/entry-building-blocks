using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using MimeKit.Utils;

namespace Enigmatry.Entry.Email
{
    public class EmailMessage
    {
        [Obsolete(
            "This constructor is obsolete. Please use one of the other ones that have a more logical argument order.")]
        public EmailMessage(string subject, string content, IEnumerable<string> to) : this(
            to.Select(address => new EmailMessageAddress(address)), subject, content)
        {
        }

        [Obsolete("This constructor is obsolete. Please use a different one that doesn't use MailAddress.")]
        public EmailMessage(string subject, string content, IEnumerable<MailAddress> to) : this(
            to.Select(address => new EmailMessageAddress(address.DisplayName, address.Address)), subject, content)
        {
        }

        public EmailMessage(string to, string subject, string body) : this(EmailMessageAddress.Empty,
            new EmailMessageAddress(to), subject, body)
        {
        }

        public EmailMessage(IEnumerable<string> to, string subject, string body) : this(EmailMessageAddress.Empty,
            to.Select(address => new EmailMessageAddress(address)), subject, body)
        {
        }

        public EmailMessage(EmailMessageAddress to, string subject, string body) : this(EmailMessageAddress.Empty,
            new[] { to }, subject, body)
        {
        }

        public EmailMessage(IEnumerable<EmailMessageAddress> to, string subject, string body) : this(
            EmailMessageAddress.Empty, to, subject, body)
        {
        }

        public EmailMessage(string from, string to, string subject, string body) : this(new EmailMessageAddress(from),
            new EmailMessageAddress(to), subject, body)
        {
        }

        public EmailMessage(string from, IEnumerable<string> to, string subject, string body) : this(
            new EmailMessageAddress(from), to.Select(address => new EmailMessageAddress(address)), subject, body)
        {
        }

        public EmailMessage(EmailMessageAddress from, EmailMessageAddress to, string subject, string body) : this(from,
            new[] { to }, subject, body)
        {
        }

        public EmailMessage(EmailMessageAddress from, IEnumerable<EmailMessageAddress> to, string subject, string body)
        {
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            Body = body ?? throw new ArgumentNullException(nameof(body));

            To.AddRange(to);
            From = from ?? throw new ArgumentNullException(nameof(from));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        }

        public EmailMessageAddress From { get; set; }
        public EmailMessageAddressCollection To { get; } = new();
        public EmailMessageAddressCollection Cc { get; } = new();
        public EmailMessageAddressCollection Bcc { get; set; } = new();
        public EmailMessageAttachmentCollection Attachments { get; set; } = new();
        public string Subject { get; }
        public string Body { get; }

        public string MessageId { get; } = MimeUtils.GenerateMessageId();

        internal IEnumerable<EmailMessage> GetBulk() =>
            To.Distinct().Select(to =>
            {
                var message = new EmailMessage(new[] { to }, Subject, Body) { From = From };
                message.Cc.AddRange(Cc);
                message.Bcc.AddRange(Bcc);
                message.Attachments.AddRange(Attachments);
                return message;
            });
    }
}
