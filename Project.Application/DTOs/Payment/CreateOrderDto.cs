namespace Project.Application.DTOs.Payment
{
    public record CreateOrderDto(decimal Amount, string Currency = "INR", string? Receipt = null);
}
