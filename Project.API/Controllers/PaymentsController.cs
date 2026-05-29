using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers.Base;
using Project.API.CustomResults;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.Payment;

namespace Project.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/payments")]
    [Authorize]
    public sealed class PaymentsController : ApiControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto, CancellationToken ct)
        {
            var result = await _paymentService.CreateOrderAsync(dto, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return StatusCode(StatusCodes.Status201Created,
                ApiResponse<OrderResponseDto>.Success(result.Value, "Order created successfully."));
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto, CancellationToken ct)
        {
            var result = await _paymentService.VerifyPaymentAsync(dto, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse<PaymentDto>.Success(result.Value, "Payment verified successfully."));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _paymentService.GetByIdAsync(id, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse<PaymentDto>.Success(result.Value));
        }
    }
}
