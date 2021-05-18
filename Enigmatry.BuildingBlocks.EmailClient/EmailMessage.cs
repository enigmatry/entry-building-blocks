using Enigmatry.BuildingBlocks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Enigmatry.BuildingBlocks.Email
{
    public class EmailMessage
    {
        private readonly ISet<MailAddress> _cc = new HashSet<MailAddress>();
        private readonly ISet<MailAddress> _to = new HashSet<MailAddress>();

        public EmailMessage(string subject, string content, IEnumerable<string> to)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            SetAddressRange(to, _to);
        }

        public EmailMessage(string subject, string content, IEnumerable<MailAddress> to)
        {
            Subject = subject;
            Content = content;
            SetAddressRange(to, _to);
        }

        public string Subject { get; }
        public string Content { get; }
        public MailAddress? From { get; set; }
        public IReadOnlyCollection<MailAddress> To => _to.ToList();
        public IReadOnlyCollection<MailAddress> Cc => _cc.ToList();

        internal void SetCc(IEnumerable<string> addresses) => SetAddressRange(addresses, _cc);

        internal void SetCc(IEnumerable<MailAddress> addresses) => SetAddressRange(addresses, _cc);

        internal IEnumerable<EmailMessage> GetBulk() =>
            _to.Select(to =>
            {
                var message = new EmailMessage(Subject, Content, new[] { to }) { From = From };
                message.SetCc(Cc);
                return message;
            });

        private static void SetAddressRange(IEnumerable<string> source, ISet<MailAddress> destination)
        {
            destination.Clear();
            source.ForEach(address => destination.Add(ParseMailAddress(address)));
        }

        private static void SetAddressRange(IEnumerable<MailAddress> source, ISet<MailAddress> destination)
        {
            destination.Clear();
            source.ForEach(address => destination.Add(address));
        }

        private static MailAddress ParseMailAddress(string mailAddress) =>
            String.IsNullOrWhiteSpace(mailAddress)
                ? throw new ArgumentException("Email address cannot be empty", nameof(mailAddress))
                : new MailAddress(mailAddress);
    }
}
