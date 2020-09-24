using System.Collections.Generic;

namespace Enigmatry.Blueprint.BuildingBlocks.Email
{
    public class EmailMessage
    {
        public EmailMessage(string subject, string content, IEnumerable<string> to, IEnumerable<string> from)
        {
            Subject = subject;
            Content = content;
            To = to;
            From = from;
        }

        public string Subject { get; }
        public string Content { get; }
        public IEnumerable<string> To { get; }
        public IEnumerable<string> From { get; }
    }
}
