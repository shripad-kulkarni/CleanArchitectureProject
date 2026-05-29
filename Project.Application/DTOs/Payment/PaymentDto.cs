namespace Project.Application.DTOs.Payment
{
    public record PaymentDto(
        int Id,
        string GatewayOrderId,
        string? GatewayPaymentId,
        decimal Amount,
        string Currency,
        string? Receipt,
        string Status,
        string? Notes);
}
