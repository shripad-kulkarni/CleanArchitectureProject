using Project.Domain.Aggregates.PaymentAggregate;

namespace Project.Application.Specifications.Payments
{
    public sealed class PaymentByOrderIdSpecification : BaseSpecification<Payment>
    {
        public PaymentByOrderIdSpecification(string gatewayOrderId)
            : base(p => p.GatewayOrderId == gatewayOrderId && !p.IsDeleted)
        {
        }
    }
}
