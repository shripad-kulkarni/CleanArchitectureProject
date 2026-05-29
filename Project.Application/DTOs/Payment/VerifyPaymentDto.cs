namespace Project.Application.DTOs.Payment
{
    public record VerifyPaymentDto(
        string GatewayOrderId,
        string GatewayPaymentId,
        string Signature);
}
