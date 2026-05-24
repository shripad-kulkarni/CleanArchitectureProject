using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Domain.ValueObjects
{
    public sealed class Email
    {
        public string Value { get; }

        private Email(string value) => Value = value;

        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException($"'{email}' is not a valid email address.");

            return new Email(email.ToLowerInvariant());
        }

        public override string ToString() => Value;
    }
}
