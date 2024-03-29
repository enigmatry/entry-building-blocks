﻿using System;
using EmailValidation;

namespace Enigmatry.Entry.Email
{
    public class EmailMessageAddress : IEquatable<EmailMessageAddress>
    {
        public static readonly EmailMessageAddress Empty = new();

        private EmailMessageAddress()
        {
            Name = string.Empty;
            Address = string.Empty;
        }

        public EmailMessageAddress(string address) : this(string.Empty, address)
        {
        }

        public EmailMessageAddress(string name, string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (!EmailValidator.Validate(address))
            {
                throw new ArgumentOutOfRangeException(nameof(address));
            }

            Address = address;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
        public string Address { get; }

        public bool Equals(EmailMessageAddress? other) =>
            other is not null &&
            (ReferenceEquals(this, other) ||
             (Name == other.Name && Address == other.Address));

        public override bool Equals(object? obj) =>
            obj is not null && (ReferenceEquals(this, obj) ||
                                (obj.GetType() == GetType() &&
                                 Equals((EmailMessageAddress)obj)));

        public override int GetHashCode() => HashCode.Combine(Name, Address);

        public override string ToString() => $"{Name} <{Address}>";
    }
}
