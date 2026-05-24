using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.ValueObjects
{
    public sealed class Address
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string PinCode { get; }
        public string Country { get; }

        private Address(string street, string city, string state, string pinCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            PinCode = pinCode;
            Country = country;
        }

        public static Address Create(string street, string city, string state, string pinCode, string country = "India")
        {
            if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street cannot be empty.");
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City cannot be empty.");
            if (string.IsNullOrWhiteSpace(state)) throw new ArgumentException("State cannot be empty.");
            if (string.IsNullOrWhiteSpace(pinCode)) throw new ArgumentException("PinCode cannot be empty.");

            return new Address(street, city, state, pinCode, country);
        }

        public override string ToString()
            => $"{Street}, {City}, {State} - {PinCode}, {Country}";
    }
}
