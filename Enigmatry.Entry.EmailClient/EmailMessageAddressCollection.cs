using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Enigmatry.Entry.Core.Helpers;

namespace Enigmatry.Entry.Email
{
    public class EmailMessageAddressCollection : ICollection<EmailMessageAddress>
    {
        private readonly Collection<EmailMessageAddress> _inner = [];

        public IEnumerator<EmailMessageAddress> GetEnumerator() => _inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(EmailMessageAddress item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _inner.Add(item);
        }

        public void Add(string address) => Add(new EmailMessageAddress(address));

        public void Clear() => _inner.Clear();

        public bool Contains(EmailMessageAddress item) => _inner.Contains(item);

        public void CopyTo(EmailMessageAddress[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);

        public bool Remove(EmailMessageAddress item) => _inner.Remove(item);

        public int Count => _inner.Count;
        public bool IsReadOnly => false;

        public void AddRange(IEnumerable<EmailMessageAddress> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            items.ForEach(item => _inner.Add(item));
        }

        public void AddRange(IEnumerable<string> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            items.ForEach(Add);
        }

        public override string ToString() => string.Join(", ", _inner);
    }
}
