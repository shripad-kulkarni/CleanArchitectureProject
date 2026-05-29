namespace Project.Application.DTOs.Payment
{
    public record OrderResponseDto(
        int PaymentId,
        string GatewayOrderId,
        decimal Amount,
        string Currency,
        string PublicKeyId);
}
