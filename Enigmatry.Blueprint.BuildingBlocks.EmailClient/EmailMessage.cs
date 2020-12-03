using Enigmatry.Blueprint.BuildingBlocks.Core.Helpers;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Enigmatry.Blueprint.BuildingBlocks.Email
{
    public class EmailMessage
    {
        [NotNull] [ItemNotNull] private readonly HashSet<MailAddress> _cc = new HashSet<MailAddress>();
        [NotNull] [ItemNotNull] private readonly HashSet<MailAddress> _to = new HashSet<MailAddress>();

        public EmailMessage([NotNull] string subject, [NotNull] string content, [NotNull] IEnumerable<string> to)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            SetAddressRange(to, _to);
        }

        public EmailMessage([NotNull] string subject, [NotNull] string content, [NotNull] IEnumerable<MailAddress> to)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            SetAddressRange(to, _to);
        }

        [NotNull] public string Subject { get; }
        [NotNull] public string Content { get; }
        [CanBeNull] public MailAddress? From { get; set; }
        [NotNull] public IEnumerable<MailAddress> To => _to;
        [NotNull] public IEnumerable<MailAddress> Cc => _cc;

        public void SetCc([NotNull] IEnumerable<string> addresses) => SetAddressRange(addresses, _cc);

        public void SetCc([NotNull] IEnumerable<MailAddress> addresses) => SetAddressRange(addresses, _cc);

        private static void SetAddressRange([NotNull] IEnumerable<string> addresses,
            [NotNull] HashSet<MailAddress> destination)
        {
            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            destination.Clear();
            addresses.Where(address => !String.IsNullOrWhiteSpace(address))
                .ForEach(a => destination.Add(new MailAddress(a)));
        }

        private static void SetAddressRange([NotNull] IEnumerable<MailAddress> source,
            [NotNull] HashSet<MailAddress> destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            destination.Clear();
            source.ForEach(a => destination.Add(a));
        }


        public IEnumerable<EmailMessage> GetBulk()
        {
            return To.Select(to =>
            {
                var message = new EmailMessage(Subject, Content, new[] { to }) { From = From };
                message.SetCc(Cc);
                return message;
            });
        }
    }
}
