using System;
using MimeKit;

namespace Enigmatry.BuildingBlocks.Email
{
    public class EmailMessageAttachment : IEquatable<EmailMessageAttachment>
    {
        public string FileName { get; set; }
        public byte[] Data { get; }
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

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FileName.GetHashCode();
                hashCode = (hashCode * 397) ^ Data.GetHashCode();
                hashCode = (hashCode * 397) ^ ContentType.GetHashCode();
                return hashCode;
            }
        }
    }
}
