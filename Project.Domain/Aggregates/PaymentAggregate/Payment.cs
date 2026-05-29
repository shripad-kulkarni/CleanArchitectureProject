using Project.Domain.Enums;
using Project.Domain.Primitives;

namespace Project.Domain.Aggregates.PaymentAggregate
{
    public sealed class Payment : AggregateRoot
    {
        private Payment() { }

        public string GatewayOrderId { get; private set; } = string.Empty;
        public string? GatewayPaymentId { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = string.Empty;
        public string? Receipt { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string? Notes { get; private set; }

        public static Payment Create(
            decimal amount,
            string currency,
            string gatewayOrderId,
            string? receipt = null)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");
            if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.");
            if (string.IsNullOrWhiteSpace(gatewayOrderId)) throw new ArgumentException("Gateway order id is required.");

            return new Payment
            {
                Amount = amount,
                Currency = currency,
                GatewayOrderId = gatewayOrderId,
                Receipt = receipt,
                Status = PaymentStatus.Pending
            };
        }

        public void MarkCaptured(string gatewayPaymentId)
        {
            if (string.IsNullOrWhiteSpace(gatewayPaymentId)) throw new ArgumentException("Gateway payment id is required.");
            GatewayPaymentId = gatewayPaymentId;
            Status = PaymentStatus.Captured;
        }

        public void MarkFailed(string? notes = null)
        {
            Status = PaymentStatus.Failed;
            Notes = notes;
        }
    }
}
