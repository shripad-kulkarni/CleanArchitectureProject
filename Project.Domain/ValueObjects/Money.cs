using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.ValueObjects
{
    public sealed class Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Create(decimal amount, string currency = "INR")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.");

            return new Money(amount, currency);
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot add different currencies.");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot subtract different currencies.");

            if (Amount < other.Amount)
                throw new InvalidOperationException("Insufficient amount.");

            return new Money(Amount - other.Amount, Currency);
        }

        public override string ToString() => $"₹{Amount:F2}";
    }
}
