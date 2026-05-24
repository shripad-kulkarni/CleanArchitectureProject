using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Domain.ValueObjects
{
    public sealed class PhoneNumber
    {
        public string Value { get; }

        private PhoneNumber(string value) => Value = value;

        public static PhoneNumber Create(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone number cannot be empty.");

            if (!Regex.IsMatch(phone, @"^\+?[0-9]{10,15}$"))
                throw new ArgumentException($"'{phone}' is not a valid phone number.");

            return new PhoneNumber(phone);
        }

        public override string ToString() => Value;
    }
}
