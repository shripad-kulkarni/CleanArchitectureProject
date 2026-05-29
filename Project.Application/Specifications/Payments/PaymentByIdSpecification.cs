using Project.Domain.Aggregates.PaymentAggregate;

namespace Project.Application.Specifications.Payments
{
    public sealed class PaymentByIdSpecification : BaseSpecification<Payment>
    {
        public PaymentByIdSpecification(int id)
            : base(p => p.Id == id && !p.IsDeleted)
        {
        }
    }
}
