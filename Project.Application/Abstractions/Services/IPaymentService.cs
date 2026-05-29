using Project.Application.Common.Result;
using Project.Application.DTOs.Payment;

namespace Project.Application.Abstractions.Services
{
    public interface IPaymentService
    {
        Task<Result<OrderResponseDto>> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default);
        Task<Result<PaymentDto>> VerifyPaymentAsync(VerifyPaymentDto dto, CancellationToken ct = default);
        Task<Result<PaymentDto>> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
