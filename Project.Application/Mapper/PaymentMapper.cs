using Project.Application.DTOs.Payment;
using Project.Domain.Aggregates.PaymentAggregate;

namespace Project.Application.Mapper
{
    public static class PaymentMapper
    {
        public static PaymentDto ToDto(Payment payment)
        {
            return new PaymentDto(
                Id: payment.Id,
                GatewayOrderId: payment.GatewayOrderId,
                GatewayPaymentId: payment.GatewayPaymentId,
                Amount: payment.Amount,
                Currency: payment.Currency,
                Receipt: payment.Receipt,
                Status: payment.Status.ToString(),
                Notes: payment.Notes);
        }
    }
}
