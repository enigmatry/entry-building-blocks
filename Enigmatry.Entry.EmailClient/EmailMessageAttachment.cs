using System;
using MimeKit;

namespace Enigmatry.Entry.Email
{
    public class EmailMessageAttachment : IEquatable<EmailMessageAttachment>
    {
        public string FileName { get; set; }
#pragma warning disable CA1819
        public byte[] Data { get; }
#pragma warning restore CA1819
        public string ContentType { get; }

        public EmailMessageAttachment(string fileName, byte[] data) : this(fileName, data,
            MimeTypes.GetMimeType(fileName))
        {
        }

        public EmailMessageAttachment(string fileName, byte[] data, string contentType)
        {
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public bool Equals(EmailMessageAttachment? other) =>
            other is not null
            && (ReferenceEquals(this, other)
                || (FileName == other.FileName && Data.Equals(other.Data) && ContentType == other.ContentType));

        public override bool Equals(object? obj) =>
            obj is not null && (ReferenceEquals(this, obj) ||
                                (obj.GetType() == GetType() &&
                                 Equals((EmailMessageAttachment)obj)));

        public override int GetHashCode() => HashCode.Combine(FileName, Data, ContentType);
    }
}
