using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Enigmatry.BuildingBlocks.Core.Helpers;

namespace Enigmatry.BuildingBlocks.Email
{
    public class EmailMessageAttachmentCollection : ICollection<EmailMessageAttachment>
    {
        private readonly Collection<EmailMessageAttachment> _inner = new();

        public IEnumerator<EmailMessageAttachment> GetEnumerator() => _inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(EmailMessageAttachment item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _inner.Add(item);
        }

        public void Add(string fileName, byte[] data, string contentType)
        {
            var attachment = new EmailMessageAttachment(fileName, data, contentType);
            Add(attachment);
        }

        public void Add(string fileName, byte[] data)
        {
            var attachment = new EmailMessageAttachment(fileName, data);
            Add(attachment);
        }

        public void AddRange(IEnumerable<EmailMessageAttachment> attachments)
        {
            if (attachments == null)
            {
                throw new ArgumentNullException(nameof(attachments));
            }

            attachments.ForEach(Add);
        }

        public void Clear() => _inner.Clear();

        public bool Contains(EmailMessageAttachment item) => _inner.Contains(item);

        public void CopyTo(EmailMessageAttachment[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);

        public bool Remove(EmailMessageAttachment item) => _inner.Remove(item);

        public int Count => _inner.Count;
        public bool IsReadOnly => false;
    }
}
