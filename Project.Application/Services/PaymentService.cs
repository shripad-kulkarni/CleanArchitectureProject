using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Application.Common.Errors;
using Project.Application.Common.Result;
using Project.Application.DTOs.Payment;
using Project.Application.Mapper;
using Project.Application.Specifications.Payments;
using Project.Domain.Aggregates.PaymentAggregate;
using Project.Domain.Enums;

namespace Project.Application.Services
{
    public sealed class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGatewayService _gateway;

        public PaymentService(
            IRepository<Payment> repository,
            IUnitOfWork unitOfWork,
            IPaymentGatewayService gateway)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _gateway = gateway;
        }

        public async Task<Result<OrderResponseDto>> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default)
        {
            if (dto.Amount <= 0)
                return Result<OrderResponseDto>.Failure(
                    Error.Validation("Payment.InvalidAmount", "Amount must be greater than zero."));

            GatewayOrderResult gatewayResult;
            try
            {
                gatewayResult = await _gateway.CreateOrderAsync(dto.Amount, dto.Currency, dto.Receipt, ct);
            }
            catch (Exception ex)
            {
                return Result<OrderResponseDto>.Failure(
                    Error.Failure("Payment.GatewayError", $"Payment gateway error: {ex.Message}"));
            }

            var payment = Payment.Create(dto.Amount, dto.Currency, gatewayResult.GatewayOrderId, dto.Receipt);
            await _repository.AddAsync(payment, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<OrderResponseDto>.Success(new OrderResponseDto(
                payment.Id,
                gatewayResult.GatewayOrderId,
                payment.Amount,
                payment.Currency,
                gatewayResult.PublicKeyId));
        }

        public async Task<Result<PaymentDto>> VerifyPaymentAsync(VerifyPaymentDto dto, CancellationToken ct = default)
        {
            var payment = await _repository.FirstOrDefaultAsync(
                new PaymentByOrderIdSpecification(dto.GatewayOrderId), ct);

            if (payment is null)
                return Result<PaymentDto>.Failure(
                    Error.NotFound("Payment.NotFound", "Payment order not found."));

            if (payment.Status == PaymentStatus.Captured)
                return Result<PaymentDto>.Failure(
                    Error.Conflict("Payment.AlreadyCaptured", "This payment has already been captured."));

            var isValid = _gateway.VerifySignature(dto.GatewayOrderId, dto.GatewayPaymentId, dto.Signature);

            if (!isValid)
            {
                payment.MarkFailed("Signature verification failed.");
                _repository.Update(payment);
                await _unitOfWork.SaveChangesAsync(ct);
                return Result<PaymentDto>.Failure(
                    Error.Validation("Payment.InvalidSignature", "Payment signature verification failed."));
            }

            payment.MarkCaptured(dto.GatewayPaymentId);
            _repository.Update(payment);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<PaymentDto>.Success(PaymentMapper.ToDto(payment));
        }

        public async Task<Result<PaymentDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var payment = await _repository.FirstOrDefaultAsync(
                new PaymentByIdSpecification(id), ct);

            if (payment is null)
                return Result<PaymentDto>.Failure(
                    Error.NotFound("Payment.NotFound", $"Payment with id {id} was not found."));

            return Result<PaymentDto>.Success(PaymentMapper.ToDto(payment));
        }
    }
}
